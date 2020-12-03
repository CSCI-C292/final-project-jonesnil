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

        this.badGuyCount = this.transform.childCount;
        int counter = 0;
        badGuys = new BadGuy[badGuyCount];

        while (counter < badGuyCount) 
        {
            badGuys[counter] = this.transform.GetChild(counter).GetComponent<BadGuy>();
            counter += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                GameEvents.InvokePlayerRespawn(badGuys[counter].transform.position);
                badGuys[counter].DieInvisible();
                return;
            }

            counter += 1;
        }
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

}
