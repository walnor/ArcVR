using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterXSeconds : MonoBehaviour
{
    public float X;

    float t = 0f;
    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (t > X)
        {
            Destroy(gameObject);
        }
    }
}
