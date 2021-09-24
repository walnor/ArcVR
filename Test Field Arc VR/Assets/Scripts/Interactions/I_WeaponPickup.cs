using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class I_WeaponPickup : Interaction
{
    public Transform WeaponToEnable;
    public Transform WeaponToDisable;

    public GameObject hoverText;

    float t = 0f;

    private void Update()
    {
        if (t > 0f)
        {
            t -= Time.deltaTime;

            hoverText.SetActive(true);
            //hover behavior;
        }else hoverText.SetActive(false);

    }
    public override void Interation()
    {

        WeaponToEnable.gameObject.SetActive(true);
        WeaponToDisable.gameObject.SetActive(false);

        hoverText.SetActive(false);
        this.enabled = false;
    }

    public override void CurserHover()
    {
        t = 0.5f;
    }

}
