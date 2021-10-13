using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    //This will probably need to be reworked


    // damage probably shouldn't be here, it should be on either the weapon or ammo being used.
    public int damage = 1;

    public ObjectHealth objHealth;

    public void OnHit()
    {
        //This function doesn't actually do enough. It should be able to interact with another function to make the object/enemy optionally react to taking damage

        objHealth.HP -= damage;
    }

}
