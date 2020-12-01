using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IntEventArgs : EventArgs
{
    public int intPayload;
}


public static class GameEvents
{

    public static event EventHandler<IntEventArgs> HeroShot;
    public static event EventHandler GameOver;
    public static event EventHandler BeatLevel;
    public static void InvokeHeroShot(int intPayloadDummy)
    {
        HeroShot(null, new IntEventArgs { intPayload = intPayloadDummy });
    }

}