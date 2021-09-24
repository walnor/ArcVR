using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public int HP = 5;
    private void Start()
    {
    }

    public void ResetHealth(int health)
    {

        //Debug.Log(StartHealth);
        HP = health;
    }
}
