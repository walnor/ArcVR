using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSingleGrabSource : MonoBehaviour
{
    public void onSelectEnter()
    {
        Collider[] Col = gameObject.GetComponents<Collider>();

        foreach (Collider c in Col)
        {
            c.enabled = false;
        }
    }
    public void onSelectExit()
    {
        Collider[] Col = gameObject.GetComponents<Collider>();

        foreach (Collider c in Col)
        {
            c.enabled = true;
        }
    }
}
