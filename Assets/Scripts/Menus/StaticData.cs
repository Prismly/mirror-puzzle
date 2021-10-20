using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StaticData
{
    private static TextAsset[] levels;
    private static int currentLevelUnlocked = 1;
    private static int levelSelected = 1;

    public static void InitializeLevelArray(TextAsset[] levelsIn)
    {
        levels = (TextAsset[])levelsIn.Clone();
    }

    public static TextAsset LayoutFromLevelNumber(int levelNum)
    {
        return levels[levelNum - 1];
    }

    public static void UnlockNextLevel()
    {
        if (levelSelected == currentLevelUnlocked && currentLevelUnlocked < levels.Length - 1)
        {
            currentLevelUnlocked++;
        }
    }

    public static void ProgressToNextLevel()
    {
        if (levelSelected + 1 >= levels.Length)
        {
            //All levels beaten, return to title screen
            SceneManager.LoadScene("Title Screen");
        }
        else
        {
            levelSelected++;
            GameGrid.SetLayout(StaticData.LayoutFromLevelNumber(levelSelected));
            SceneManager.LoadScene("Game Scene");
        }
    }

    public static int GetCurrentLevelUnlocked()
    {
        return currentLevelUnlocked;
    }

    public static void SetCurrentLevelUnlocked(int newNum)
    {
        currentLevelUnlocked = newNum;
    }

    public static int GetLevelSelected()
    {
        return levelSelected;
    }

    public static void SetLevelSelected(int newNum)
    {
        levelSelected = newNum;
    }
}
