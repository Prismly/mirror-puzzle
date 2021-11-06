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

    private static KeyCode debugCode = KeyCode.Backslash;

    [SerializeField] private Tilemap bg1;
    [SerializeField] private Tilemap bg2;
    [SerializeField] private Sprite[] bgSprites;

    public void Start()
    {
        StaticData.InitializeLevelArray(levels);
        UpdateTextComponents();
    }

    public void Update()
    {
        if(Input.GetKeyDown(debugCode))
        {
            StaticData.UnlockAllLevels();
        }
        UpdateTextComponents();
    }

    public void ResetProgress()
    {
        SoundManager.PlaySound("click");
        StaticData.SetLevelSelected(1);
        StaticData.SetCurrentLevelUnlocked(1);
        UpdateTextComponents();
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
        SoundManager.PlaySound("click");
        StaticData.SetLevelSelected(result);
        
    }

    public void LoadLevel(int levelNum)
    {
        SoundManager.PlaySound("click");
        GameGrid.SetLayout(StaticData.LayoutFromLevelNumber(levelNum));
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void LoadSelectedLevel()
    {
        SoundManager.PlaySound("click");
        LoadLevel(StaticData.GetLevelSelected());
    }

    private void UpdateTextComponents()
    {
        continueFromButton.GetComponentInChildren<Text>().text = "Continue From: " + StaticData.GetLevelSelected();
        maxULText.GetComponent<Text>().text = "Current Level: " + StaticData.GetCurrentLevelUnlocked();
    }
}
