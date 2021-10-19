using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    private static int currentLevelUnlocked = 0;

    public static int GetCurrentLevelUnlocked()
    {
        return currentLevelUnlocked;
    }

    public static void SetCurrentLevelUnlocked(int newNum)
    {
        currentLevelUnlocked = newNum;
    }
}
