using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_FirstButton : Interaction
{
    public Interaction ToCall;

    public void Update()
    {
    }
    public override void Interation()
    {
        ToCall.Interation();
        //Debug.Log("Button Clicked!");
    }
}
