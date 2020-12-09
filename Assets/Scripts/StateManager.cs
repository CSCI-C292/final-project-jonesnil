using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateManager : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    BadGuy[] badGuys;
    int badGuyCount;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.PlayerDead += OnPlayerDead;
        GameEvents.GameOver += OnGameOver;
        GameEvents.BadGuyDead += OnBadGuyDead;

        this.badGuyCount = this.transform.childCount;
        int counter = 0;
        badGuys = new BadGuy[badGuyCount];

        while (counter < badGuyCount) 
        {
            badGuys[counter] = this.transform.GetChild(counter).GetComponent<BadGuy>();
            counter += 1;
        }

        data.nearestBadGuyToHero = NearestBadGuyToHero();
    }


    void OnPlayerDead(object sender, EventArgs args) 
    {
        int counter = 0;

        while (counter < badGuyCount)
        {
            if (!badGuys[counter].dead) 
            {
                badGuys[counter].nearest = false;
                
                Vector3 playerSpawnPos = badGuys[counter].transform.position;
                Debug.Log(playerSpawnPos);
                Vector3 playerPos = Camera.main.transform.position;
                badGuys[counter].DieAtPosition(playerPos);
                GameEvents.InvokePlayerRespawn(playerSpawnPos);
                data.nearestBadGuyToHero = NearestBadGuyToHero();
                return;
            }

            counter += 1;
        }

        GameEvents.InvokeGameOver();
    }

    Vector3 NearestBadGuyToHero() 
    {
        Vector3 nearest = new Vector3(0, 0, 0);
        int counter = 0;

        while (counter < badGuyCount)
        {
            if (!badGuys[counter].dead)
            {
                badGuys[counter].nearest = true;
                nearest = badGuys[counter].transform.position;
                return nearest;
            }
            counter += 1;
        }

        return nearest;
    }

    void OnBadGuyDead(object sender, EventArgs args)
    {
        data.nearestBadGuyToHero = NearestBadGuyToHero();
    }

    void OnGameOver(object sender, EventArgs args)
    {
        GameEvents.PlayerDead -= OnPlayerDead;
        GameEvents.GameOver -= OnGameOver;
        GameEvents.BadGuyDead -= OnBadGuyDead;
    }

}
