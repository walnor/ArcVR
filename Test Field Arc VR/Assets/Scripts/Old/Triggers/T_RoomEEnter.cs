using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_RoomEEnter : MonoBehaviour
{
    //public Interaction ToActivate;

    public LayerMask interactableLayer;

    float t = 0;

    //bool active = false;

    public GameObject boss;
    public GameObject EndText;

    ObjectHealth bossHp;

    private void Start()
    {
        //Zombies.SetActive(false);

        bossHp = boss.GetComponent<ObjectHealth>();
    }

    private void Update()
    {
        if (bossHp.HP <= 0)
        {
            EndText.SetActive(true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int layermask = other.gameObject.layer;

        if (layermask == (layermask | (1 << interactableLayer)))
        {
            //active = true;
            GetComponent<Collider>().enabled = false;

            boss.SetActive(true);
            boss.GetComponent<BasicZombieController>().ZomStart();
        }
    }
}
