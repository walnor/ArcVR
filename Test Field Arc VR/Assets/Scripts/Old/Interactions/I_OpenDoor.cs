using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_OpenDoor : Interaction
{
    public Transform door;
    public Transform MoveTo;

    public float AniTime = 1f;

    bool activate = false;

    Vector3 startPostion;
    Vector3 target;

    float t;
    private void Awake()
    {
        t = 0;
        startPostion = door.position;
        target = MoveTo.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (activate)
        {
            t += Time.deltaTime / AniTime;
            if (t >= 1f)
            {
                t = 1;
                activate = false;
            }
            door.position = Vector3.Lerp(startPostion, target, t);
        }
    }
    public override void Interation()
    {
        activate = true;
    }
}
