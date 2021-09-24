using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class I_APICallRoomD : Interaction
{
    public string URL = "http://worldtimeapi.org/api/timezone/America/Chicago";

    public Text responseText;

    public override void Interation()
    {
        Request();
    }
    public void Request()
    {
        WWW request = new WWW(URL);
        StartCoroutine(OnResponse(request));
    }

    private IEnumerator OnResponse(WWW req)
    {
        yield return req;

        List<string> Stringlist = req.text.Split(',').ToList();

        string DateTime = "";

        foreach (string s in Stringlist)
        {
            if (s.Contains("datetime"))
            {
                DateTime = s.Remove(0, 11);
                DateTime = DateTime.Replace("\"", "");
                break;
            }
        }

        responseText.text = "Date Time: \n" + DateTime;
    }
}
