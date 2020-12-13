using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateManager : MonoBehaviour
{
    [SerializeField] RunTimeData data;
    BadGuy[] badGuys;
    int badGuyCount;

    // This class keeps track of how many bad guys are in the game (they are children of
    // this game object) and also which bad guy is nearest to the hero. Because the hero 
    // has to go to the core in a line through the base, I thought the hero only really 
    // needs to be trying to shoot the player/the closest bad guy at any given time. It's
    // also helpful to keep track of the nearest bad guy so the player can respawn as them 
    // when they die.

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.PlayerDead += OnPlayerDead;
        GameEvents.GameOver += OnGameOver;
        GameEvents.BadGuyDead += OnBadGuyDead;

        // Gets all children of this and sets them each as bad guys (in order of their
        // index as children, which is also their order through the map from the hero to
        // the core)
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

    // When the player dies, kill the nearest bad guy to the hero and spawn the player at that location

    void OnPlayerDead(object sender, EventArgs args) 
    {
        int counter = 0;

        while (counter < badGuyCount)
        {
            if (!badGuys[counter].dead) 
            {
                badGuys[counter].nearest = false;
                
                Vector3 playerSpawnPos = badGuys[counter].transform.position;
                Vector3 playerPos = Camera.main.transform.position;
                badGuys[counter].DieAtPosition(playerPos);
                GameEvents.InvokePlayerRespawn(playerSpawnPos);
                data.nearestBadGuyToHero = NearestBadGuyToHero();
                return;
            }

            counter += 1;
        }

        GameEvents.InvokeGameOver(false);
    }

    // Recalculates the nearest bad guy (called every time that should change, like when a bad guy
    // is killed by the hero or the player respawning)
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

    // Tells this to recalculate the nearest bad guy every time one dies (and they should die
    // in order every time)
    void OnBadGuyDead(object sender, EventArgs args)
    {
        data.nearestBadGuyToHero = NearestBadGuyToHero();
    }

    // Disconnect events so game can reload properly
    void OnGameOver(object sender, BoolEventArgs args)
    {
        GameEvents.PlayerDead -= OnPlayerDead;
        GameEvents.GameOver -= OnGameOver;
        GameEvents.BadGuyDead -= OnBadGuyDead;
    }

}
