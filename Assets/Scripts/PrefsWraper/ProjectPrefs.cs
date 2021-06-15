using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectPrefs 
{
    public static IntPref CurrentLvl;
    public static IntPref TotalPoints;

    static ProjectPrefs()
    {
        CurrentLvl = new IntPref("CurrentLvl", 1);
        TotalPoints = new IntPref("TotalPoints", 0);
    }
}
