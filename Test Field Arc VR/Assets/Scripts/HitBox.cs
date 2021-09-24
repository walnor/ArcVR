using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int damage = 1;

    public ObjectHealth objHealth;

    public void OnHit()
    {
        objHealth.HP -= damage;
    }

}
