using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float rayDistance = 5;
    public float raySphereRadius = 5;

    public LayerMask interactableLayer;

    public Camera m_cam;

    private Interactable TargetInteraction = null;

    private void Awake()
    {
        m_cam = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        CheckForInteratable();
        CheckForInteractableInput();
    }

    void CheckForInteratable()
    {
        Ray ray = new Ray(m_cam.transform.position, m_cam.transform.forward);
        RaycastHit hitInfo;

        bool hitSomething = Physics.SphereCast(ray, raySphereRadius, out hitInfo, rayDistance, interactableLayer);

        if (hitSomething)
        {
            Interactable interactable = hitInfo.transform.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (TargetInteraction == null && hitInfo.distance <= interactable.InteractionDistance)
                    TargetInteraction = interactable;
            }
            else TargetInteraction = null;
        }
        else
        {
            TargetInteraction = null;
        }

        Debug.DrawRay(ray.origin, ray.direction, hitSomething ? Color.green : Color.red);
    }

    void CheckForInteractableInput()
    {
        if (TargetInteraction != null)
        { TargetInteraction.InteractUpdate(); }
    }
}
