using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_RoomBEnter : MonoBehaviour
{
    public Interaction ToActivate;

    public LayerMask interactableLayer;

    public float delay = 30f;

    float t = 0;

    bool active = false;

    private void Update()
    {
        if (active)
        {
            t += Time.deltaTime;

            if (t >= delay)
            {
                ToActivate.Interation();

                active = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int layermask = other.gameObject.layer;

        if (layermask == (layermask | (1 << interactableLayer)))
        {
            active = true;
            GetComponent<Collider>().enabled = false;
        }
    }
}
