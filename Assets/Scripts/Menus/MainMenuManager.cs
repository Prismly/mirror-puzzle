using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private string gameplaySceneName = "Game Scene";
    [SerializeField] private TextAsset[] levels;
    [SerializeField] private Text titleText;
    [SerializeField] private Text maxULText;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject continueFromButton;
    [SerializeField] private GameObject advanceLeftButton;
    [SerializeField] private GameObject advanceRightButton;
    [SerializeField] private GameObject resetButton;

    [SerializeField] private Tilemap bg1;
    [SerializeField] private Tilemap bg2;
    [SerializeField] private Sprite[] bgSprites;

    public void Start()
    {
        StaticData.InitializeLevelArray(levels);
        continueFromButton.GetComponentInChildren<Text>().text = "Continue From: " + StaticData.GetLevelSelected();
        maxULText.text = "Next Level: " + StaticData.GetCurrentLevelUnlocked();
    }

    public void ResetProgress()
    {
        StaticData.SetLevelSelected(1);
        StaticData.SetCurrentLevelUnlocked(1);
        continueFromButton.GetComponentInChildren<Text>().text = "Continue From: " + StaticData.GetCurrentLevelUnlocked();
    }

    public void IncrementSelectedLevel(int diff)
    {
        int result = StaticData.GetLevelSelected() + diff;
        if(result <= 1)
        {
            result = 1;
        }
        else if(result >= StaticData.GetCurrentLevelUnlocked())
        {
            result = StaticData.GetCurrentLevelUnlocked();
        }

        StaticData.SetLevelSelected(result);
        continueFromButton.GetComponentInChildren<Text>().text = "Continue From: " + StaticData.GetLevelSelected();
    }

    public void LoadLevel(int levelNum)
    {
        GameGrid.SetLayout(StaticData.LayoutFromLevelNumber(levelNum));
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadSelectedLevel()
    {
        LoadLevel(StaticData.GetLevelSelected());
    }
}
