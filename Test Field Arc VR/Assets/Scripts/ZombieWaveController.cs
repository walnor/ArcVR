using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieWaveController : MonoBehaviour
{
    public BasicZombieController[] Wave1;
    public BasicZombieController[] Wave2;
    public BasicZombieController[] Wave3;
    public BasicZombieController[] Wave4;
    public BasicZombieController[] Wave5;

    int WaveIndex = 0;

    public bool BeginWaves = false;

    public Interaction EndWavesInteraction;
    void Start()
    {
        DeactivateAllZoms();
    }

    // Update is called once per frame
    void Update()
    {
        if (BeginWaves)
        {
            BeginWaves = false;
            WaveStart(Wave1);
            WaveIndex++;
        }

        switch (WaveIndex)
        {
            case 0:
                break;
            case 1:
                WaveCheck(Wave1, Wave2);
                break;
            case 2:
                WaveCheck(Wave2, Wave3);
                break;
            case 3:
                WaveCheck(Wave3, Wave4);
                break;
            case 4:
                WaveCheck(Wave4, Wave5);
                break;
            case 5:
                WaveCheck(Wave5);
                break;
            case 6:
                EndRoomGame();
                break;
        }

        debugSkipRoom();
    }

    void EndRoomGame()
    {
        //open door?
        EndWavesInteraction.Interation();

        gameObject.SetActive(false);
    }

    void WaveCheck(BasicZombieController[] Wave)
    {
        foreach (BasicZombieController zom in Wave)
        {
            if (zom.isActiveAndEnabled)
            {
                return;
            }
        }
        WaveIndex++;
    }

    void WaveCheck(BasicZombieController[] Wave, BasicZombieController[] NextWave)
    {
        foreach (BasicZombieController zom in Wave)
        {
            if (zom.isActiveAndEnabled)
            {
                return;
            }
        }
        WaveIndex++;

        //Debug.Log("nextWave");
        WaveStart(NextWave);
    }

    private void WaveStart(BasicZombieController[] wave)
    {
        foreach (BasicZombieController zom in wave)
        {
            zom.gameObject.SetActive(true);
            zom.Spawn();
        }
    }

    void DeactivateAllZoms()
    {
        foreach (BasicZombieController zom in Wave1)
        {
            zom.gameObject.SetActive(false);
        }
        foreach (BasicZombieController zom in Wave2)
        {
            zom.gameObject.SetActive(false);
        }
        foreach (BasicZombieController zom in Wave3)
        {
            zom.gameObject.SetActive(false);
        }
        foreach (BasicZombieController zom in Wave4)
        {
            zom.gameObject.SetActive(false);
        }
        foreach (BasicZombieController zom in Wave5)
        {
            zom.gameObject.SetActive(false);
        }
    }

    void debugSkipRoom()
    {
        if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.L))
        {
            DeactivateAllZoms();
            EndRoomGame();
        }
    }
}
