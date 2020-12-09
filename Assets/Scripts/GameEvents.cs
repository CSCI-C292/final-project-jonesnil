using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IntEventArgs : EventArgs
{
    public int intPayload;
}

public class  PositionEventArgs : EventArgs
{
    public Vector3 positionPayload;
}


public static class GameEvents
{

    public static event EventHandler<IntEventArgs> HeroShot;
    public static event EventHandler PlayerShot;
    public static event EventHandler PlayerDead;
    public static event EventHandler<PositionEventArgs> PlayerRespawn;
    public static event EventHandler NearestBadGuyShot;
    public static event EventHandler GameOver;
    public static void InvokeHeroShot(int intPayloadDummy)
    {
        HeroShot(null, new IntEventArgs { intPayload = intPayloadDummy });
    }

    public static void InvokePlayerShot()
    {
        PlayerShot(null, EventArgs.Empty);
    }

    public static void InvokePlayerDead()
    {
        PlayerDead(null, EventArgs.Empty);
    }

    public static void InvokePlayerRespawn(Vector3 position)
    {
        PlayerRespawn(null, new PositionEventArgs { positionPayload = position });
    }

    public static void InvokeNearestBadGuyShot()
    {
        NearestBadGuyShot(null, EventArgs.Empty);
    }

    public static void InvokeGameOver()
    {
        GameOver(null, EventArgs.Empty);
    }

}