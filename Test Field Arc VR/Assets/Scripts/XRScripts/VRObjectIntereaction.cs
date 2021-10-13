using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRObjectIntereaction : MonoBehaviour
{

    public UnityEvent TriggerEvent;
    public UnityEvent SecondaryButtonEvent;

    private HandPresence ActiveHand;

    public void objSelectedEnter()
    {
        Debug.Log("This object has been selected");

        ActiveHand = GetHandPresence();

        if (ActiveHand != null)
        {
            Debug.Log("Found correct hand");

            ActiveHand.TriggerEvent.AddListener(TriggerEventAction);
            ActiveHand.SencondaryButtonEvents.AddListener(SecondaryButtonEventAction);

            ActiveHand.TurnInvisable();
        }
    }
    public void objSelectedExit()
    {
        Debug.Log("This object has been unselected");

        if (ActiveHand != null)
        {
            ActiveHand.TriggerEvent.RemoveListener(TriggerEventAction);
            ActiveHand.SencondaryButtonEvents.RemoveListener(SecondaryButtonEventAction);
            ActiveHand.TurnVisable();

            ActiveHand = null;
        }
    }

    HandPresence GetHandPresence()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, .5f);

        List<HandPresence> hands = new List<HandPresence>();

        foreach (var hitCollider in hitColliders)
        {
            HandPresence hp = hitCollider.GetComponent<HandPresence>();

            if (hp != null)
            {
                hands.Add(hp);
            }
        }

        HandPresence toReturn = null;

        if (hands.Count >= 1)
        {
            toReturn = hands[0];
        }

        foreach (HandPresence hp in hands)
        {
            float distanceA = Vector3.Distance(transform.position, toReturn.transform.position);
            float distanceB = Vector3.Distance(transform.position, hp.transform.position);

            if (distanceB < distanceA)
                toReturn = hp;
        }

        return toReturn;
    }

    public void TriggerEventAction()
    {
        TriggerEvent.Invoke();
    }

    public void SecondaryButtonEventAction()
    {
        SecondaryButtonEvent.Invoke();
    }

}
