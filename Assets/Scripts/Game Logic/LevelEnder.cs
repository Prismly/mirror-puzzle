using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnder : MonoBehaviour
{
    [SerializeField] bool isWin;
    private KeyCode continueToNextLevel = KeyCode.Return;

    void Update()
    {
        if(isWin && Input.GetKeyDown(continueToNextLevel))
        {
            StaticData.ProgressToNextLevel();
        }
        else if(Input.GetKeyDown(Player.undo))
        {
            GameGrid.SetLevelComplete(false);
            Destroy(gameObject);
        }
    }
}
