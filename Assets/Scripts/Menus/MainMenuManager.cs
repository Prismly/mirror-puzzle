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
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button levelSelectButton;

    [SerializeField] private Tilemap bg1;
    [SerializeField] private Tilemap bg2;
    [SerializeField] private Sprite[] bgSprites;

    public void Start()
    {
        continueButton.GetComponentInChildren<Text>().text = "Continue from: " + StaticData.GetCurrentLevelUnlocked();
    }

    public void LoadLevel(int levelNum)
    {
        GameGrid.SetLayout(levels[levelNum]);
        SceneManager.LoadScene(gameplaySceneName);
    }
}
