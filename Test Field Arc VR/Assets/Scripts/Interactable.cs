using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    bool interactClick = false;
    bool interactRelease = false;

    public float HoldDuration = .5f;

    public bool HoldInteract = false;
    public bool MultipleUse = true;
    public bool IsInteractable = true;

    public Interaction TargetInteraction;

    public float InteractionDistance = 1f;

    float holding = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InteractUpdate()
    {
        if (!IsInteractable)
            return;

        TargetInteraction.CurserHover();

        interactClick = Input.GetButton("Submit");

        interactRelease = Input.GetButtonUp("Submit");

        if (interactClick)
            holding += Time.deltaTime;

        if (interactRelease)
        {
            if (!HoldInteract || holding >= HoldDuration)
            {
                OnInteraction();
            }
        }

        if (!interactClick)
            holding = 0f;
    }

    void OnInteraction()
    {
        TargetInteraction.Interation();

        if (!MultipleUse)
        {
            IsInteractable = false;
        }
    }
}
