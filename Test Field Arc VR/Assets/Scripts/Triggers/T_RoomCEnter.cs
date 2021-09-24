using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_RoomCEnter : MonoBehaviour
{
    //public Interaction ToActivate;

    public LayerMask interactableLayer;

    float t = 0;

    //bool active = false;

    public ZombieWaveController Zombies;

    private void Start()
    {
        //Zombies.SetActive(false);
    }

    private void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        int layermask = other.gameObject.layer;

        if (layermask == (layermask | (1 << interactableLayer)))
        {
            //active = true;
            GetComponent<Collider>().enabled = false;

            Zombies.BeginWaves = true;
        }
    }
}
