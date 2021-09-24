using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Util;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace UnityEditor.Searcher
{
    public class LuceneSearcherDatabase : SearcherDatabaseBase
    {
        /*
         * Lucene 101:
         * - THERE'S NO MAGIC
         * - a lucene db is a collection of documents
         * - each document has N fields
         * - each field can be:
         *     - indexed: used in search
         *     - if indexed, tokenized: used in search, but processed first (eg. lowercase all words, split phrase in words then index them separately)
         *     - stored: the value sent can be retrieved. the one thing we need to STORE is the ID, we don't need to STORE the doc or the title as we can retrieve them on the C# side from the searcher item, using the ID as key
         * - the indexed doc and the query must match. if indexing lowercases everything but the query does not, nothing will match
         * - tokenization is done by analyzers. the standard analyzer, by default, wants to strip "stop words" (the, a, for, ...). we don't want that
         * - fields can be of type Int. to search for field == 42, you actually need to do a numeric range query: field in range [42,42]
         */

        public enum FilterType : byte
        {
            Must,
            MustNot,
        }

        public struct Filter
        {
            public string Field;
            public object Value;
            public FilterType Type;
        }

        class VSDocAnalyzer : Analyzer
        {
            public const int k_MinGramSize = 3;
            public const int k_MaxGramSize = 5;
            private LuceneVersion version;

            public VSDocAnalyzer(LuceneVersion version)
            {
                this.version = version;
            }

            protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
            {
                // in order: std tokenized
                Tokenizer source = new StandardTokenizer(version, reader);
                // lowercase
                TokenStream tokenStream = new LowerCaseFilter(version, source);
                // replace non-ascii (eg. unicode) chars by the closest ascii equivalent if possible
                tokenStream = new ASCIIFoldingFilter(tokenStream);
                // creates all ngrams
                tokenStream = new NGramTokenFilter(version, tokenStream, k_MinGramSize, k_MaxGramSize);
                return new TokenStreamComponents(source, tokenStream);
            }
        }

        private Directory fsDirectory;

        private List<Filter> m_Filters;

        // title: "on update"
        private const string k_TitleField = "Title";

        // title, but indexed without space: "onupdate"
        private const string k_TitleNoSpacesField = "TitleNoSpaces";

        // id from SearcherItem
        private const string k_IdField = "ItemId";

        // doc (also contains the title to allow title/doc mixed search, eg "on update <sentence from the on update doc>")
        private const string k_DocField = "Doc";

        private LuceneSearcherDatabase(List<SearcherItem> items) : base("")
        {
            m_ItemList = new List<SearcherItem>();
            int nextId = 0;
            foreach (var item in items)
                AddItemToIndex(item, ref nextId, null);
        }

        public override List<SearcherItem> Search(string query, out float localMaxScore)
        {
            Query q = null;
            if (string.IsNullOrWhiteSpace(query))
            {
                q = MakeFilterQuery(true);
            }
            else
            {
                query = query.ToLowerInvariant();
                // title: split prefixes (on update indexed as 'on' 'update' so 'on* up*' would match)
                var prefixQuery = new BooleanQuery() {Boost = 10};

                // search in doc field
                var docQuery = new NGramPhraseQuery(VSDocAnalyzer.k_MinGramSize);

                foreach (var queryPart in query.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    // add 'on*' to the split prefix query
                    prefixQuery.Add(new PrefixQuery(new Term(k_TitleField, queryPart)), Occur.MUST);
                    // adding a doc query longer then the maxgramsize gives no results
                    // doc field is indexed with 3 to 5 ngrams. meaning "update" -> "updat", "xyz" -> "xyz", "xy" -> not indexed
                    // we need to substring(maxGramSize), otherwise a query with "update" would not find any 3to5gram as it doesn't match the indexed "updat"
                    docQuery.Add(new Term(k_DocField, queryPart.Length > VSDocAnalyzer.k_MaxGramSize
                        ? queryPart.Substring(0, VSDocAnalyzer.k_MaxGramSize)
                        : queryPart));
                }

                var bq = new BooleanQuery
                {
                    // split prefixes
                    {prefixQuery, Occur.SHOULD},
                    // no space abbrev prefix query (on update indexed as 'onupdate' so 'onup*' would match)
                    {new PrefixQuery(new Term(k_TitleNoSpacesField, query)) {Boost = 10}, Occur.SHOULD},
                    {docQuery, Occur.SHOULD}
                };

                // means at least one part of bq must occur. seems to be required ?
                var fq = MakeFilterQuery(false);
                fq.Add(bq, Occur.MUST);
                q = fq;
            }

            if (q == null)
            {
                localMaxScore = 0;
                return new List<SearcherItem>();
            }

            using (var directoryReader = DirectoryReader.Open(fsDirectory))
            {
                List<SearcherItem> results = new List<SearcherItem>();

                var searcher = new IndexSearcher(directoryReader);
                TopDocs search = searcher.Search(q, 10000);
                localMaxScore = search.MaxScore;

                ISet<string> fieldSet = new HashSet<string> {k_IdField};
                foreach (ScoreDoc hit in search.ScoreDocs)
                {
                    Document foundDoc = searcher.Doc(hit.Doc, fieldSet);
                    StoredField id = (StoredField) foundDoc.GetField(k_IdField);
                    if (id == null)
                        continue;
                    results.Add(m_ItemList[id.GetInt32Value().Value]);
                }

                return results;
            }

            BooleanQuery MakeFilterQuery(bool matchAll)
            {
                var filterQuery = new BooleanQuery();
                if (m_Filters == null || m_Filters.Count == 0 || matchAll)
                    filterQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
                if (m_Filters != null)
                    foreach (var filter in m_Filters)
                    {
                        if (filter.Value == null)
                            continue;
                        var filterClauseQuery = filter.Value is int i
                            ? (Query) NumericRangeQuery.NewInt32Range(filter.Field, i, i, true, true)
                            : new TermQuery(new Term(filter.Field, filter.Value.ToString()));
                        filterQuery.Add(new BooleanClause(filterClauseQuery,
                            filter.Type == FilterType.Must ? Occur.MUST : Occur.MUST_NOT));
                    }

                return filterQuery;
            }
        }

        public static LuceneSearcherDatabase Create(List<SearcherItem> items,
            Dictionary<SearcherItem, IEnumerable<IIndexableField>> extraFields)
        {
            var luceneSearcherDatabase = new LuceneSearcherDatabase(items);

            var AppLuceneVersion = LuceneVersion.LUCENE_48;

            luceneSearcherDatabase.fsDirectory = new RAMDirectory();

            // create an analyzer to process the text
            var standardAnalyzer = new StandardAnalyzer(AppLuceneVersion, CharArraySet.EMPTY_SET);
            // NO stop words. we want to keep them for the title (so "wait for all triggers" is not indexed as "wait triggers")
            Analyzer analyzer = new PerFieldAnalyzerWrapper(standardAnalyzer,
                new J2N.Collections.Generic.Dictionary<string, Analyzer>
                {
                    {
                        // id field indexed as-is, no processing. that's what the keyword analyzer is made for
                        k_IdField, new KeywordAnalyzer()
                    },
                    {
                        k_DocField, new VSDocAnalyzer(AppLuceneVersion)
                    }
                });

            // reuse fields/doc
            var idField = new StoredField(k_IdField, 0);
            var nameField = new TextField(k_TitleField, "", Field.Store.NO);
            var nameNoSpacesField = new TextField(k_TitleNoSpacesField, "", Field.Store.NO);
            var docField = new TextField(k_DocField, "", Field.Store.NO);
            var document = new Document()
            {
                idField,
                nameField,
                nameNoSpacesField,
                docField,
            };

            // create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            var writer = new IndexWriter(luceneSearcherDatabase.fsDirectory, indexConfig);
            Rec(items);
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
            writer.Dispose();
            return luceneSearcherDatabase;

            void Rec(List<SearcherItem> searcherItems)
            {
                foreach (var item in searcherItems)
                {
                    if (item.HasChildren)
                    {
                        Rec(item.Children);
                        // continue;
                    }

                    idField.SetInt32Value(item.Id);
                    nameField.SetStringValue(item.Path);
                    nameNoSpacesField.SetStringValue(item.Name.Replace(" ", null));
                    docField.SetStringValue(string.IsNullOrWhiteSpace(item.Help)
                        ? ""
                        : (item.Name + " " + (item.Help ?? "")));

                    if (extraFields != null && extraFields.TryGetValue(item, out var fields) && fields != null)
                    {
                        document.Fields.AddRange(fields);

                        writer.AddDocument(document);

                        foreach (var indexableField in fields)
                            document.Fields.Remove(indexableField);
                    }
                    else
                        writer.AddDocument(document);
                }
            }
        }

        public void SetFilters(List<Filter> luceneFilters)
        {
            m_Filters = luceneFilters;
        }
    }
}
