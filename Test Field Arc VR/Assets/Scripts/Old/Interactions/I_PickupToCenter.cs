using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_PickupToCenter : Interaction
{

    public Transform pickupPoint;
    Transform OldParrent;

    public bool PickedUp = false;

    public float MovePower = 5f;

    private void Update()
    {
        if (PickedUp)
        {

            float step = MovePower * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, pickupPoint.position, step);
        }
    }

    public override void Interation()
    {
        if (!PickedUp)
        {
            OldParrent = transform.parent;
            PickedUp = true;
            transform.parent = pickupPoint;

            GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            //OldParrent = transform.parent;
            PickedUp = false;
            transform.parent = OldParrent;

            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
