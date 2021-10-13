using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Magazin : MonoBehaviour
{
    public int MaxCapacity = 8;

    private int Bullets = 8;

    public bool StartFull = true;

    public Collider MagCollider;
    public Rigidbody MagRig;
    public XRGrabInteractable MagGrab;

    private Transform objParrent = null;

    private void Start()
    {
        if (StartFull)
        {
            Bullets = MaxCapacity;
        }
        else 
        {
            Bullets = 0;
        }
    }

    private void Update()
    {
        if (objParrent != null)
        {
            Attach(objParrent);
        }
    }

    public void Eject()
    {
        transform.parent = null;

        MagCollider.enabled = true;

        MagRig.isKinematic = false;

        MagGrab.enabled = true;

        objParrent = null;
    }

    public void Attach(Transform NewParrent)
    {
        transform.parent = NewParrent;

        MagCollider.enabled = false;

        //MagGrab.selectExited.Invoke(new SelectExitEventArgs());

        MagGrab.enabled = false;

        MagRig.isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        objParrent = NewParrent;

    }

    //If bullets remaining, reduce bullets by one and return true; else return false
    public bool GiveBullet()
    {
        if (Bullets > 0)
        {
            Bullets--;

            return true;
        }
        return false;
    }

    public void reloadToMax()
    {
        Bullets = MaxCapacity;
    }
}
