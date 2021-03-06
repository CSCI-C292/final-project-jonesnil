﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New RunTimeData")]

public class RunTimeData : ScriptableObject
{

    public Vector3 heroPos;
    public Vector3 nearestBadGuyToHero;
    public int heroDamage;
}