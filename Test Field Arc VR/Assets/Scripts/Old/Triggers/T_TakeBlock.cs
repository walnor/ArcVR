using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_TakeBlock : MonoBehaviour
{
    public Transform acceptedObject;
    public Interaction PickupMoveInto;

    bool active = false;

    float MovePower = 5f;

    float time = 0f;

    Quaternion startRotation;
    private void Update()
    {
        if (active)
        {
            float step = MovePower * Time.deltaTime; // calculate distance to move
            time += step;
            acceptedObject.position = Vector3.MoveTowards(acceptedObject.position, transform.position, step);

            acceptedObject.rotation = Quaternion.Lerp(startRotation, Quaternion.Euler(0, 0, 0), time);

            if (Vector3.Distance(acceptedObject.position, gameObject.transform.position) <= 0.001f &&
                Vector3.Distance(acceptedObject.rotation.eulerAngles, Quaternion.Euler(0, 0, 0).eulerAngles) <= 0.001f)
            {
                //Debug.Log("Done");
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == acceptedObject.gameObject)
        {
            acceptedObject.parent = transform.parent;

            acceptedObject.gameObject.GetComponent<Interactable>().IsInteractable = false;
            acceptedObject.gameObject.GetComponent<I_PickupToCenter>().PickedUp = false;
            acceptedObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            active = true;

            startRotation = acceptedObject.rotation;
            //Any Extra Triggers
            PickupMoveInto.Interation();
        }
    }
}
