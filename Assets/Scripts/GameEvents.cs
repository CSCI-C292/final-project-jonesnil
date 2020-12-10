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

public class BoolEventArgs : EventArgs
{
    public bool boolPayload;
}


public static class GameEvents
{

    public static event EventHandler<IntEventArgs> HeroShot;
    public static event EventHandler PlayerShot;
    public static event EventHandler PlayerDead;
    public static event EventHandler<PositionEventArgs> PlayerRespawn;
    public static event EventHandler NearestBadGuyShot;
    public static event EventHandler BadGuyDead;
    public static event EventHandler<BoolEventArgs> GameOver;
    public static void InvokeHeroShot(int intPayloadDummy)
    {
        if (HeroShot != null)
            HeroShot(null, new IntEventArgs { intPayload = intPayloadDummy });
    }

    public static void InvokePlayerShot()
    {
        if(PlayerShot != null)
            PlayerShot(null, EventArgs.Empty);
    }

    public static void InvokePlayerDead()
    {
        if (PlayerDead != null)
            PlayerDead(null, EventArgs.Empty);
    }

    public static void InvokePlayerRespawn(Vector3 position)
    {
        if (PlayerRespawn != null)
            PlayerRespawn(null, new PositionEventArgs { positionPayload = position });
    }

    public static void InvokeNearestBadGuyShot()
    {
        if (NearestBadGuyShot != null)
            NearestBadGuyShot(null, EventArgs.Empty);
    }

    public static void InvokeBadGuyDead()
    {
        if (BadGuyDead != null)
            BadGuyDead(null, EventArgs.Empty);
    }

    public static void InvokeGameOver(bool wonGame)
    {
        if (GameOver != null)
            GameOver(null, new BoolEventArgs {boolPayload = wonGame});
    }


}