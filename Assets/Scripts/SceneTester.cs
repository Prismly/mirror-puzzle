using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneTester : MonoBehaviour
{
    [SerializeField] Tilemap groundMap;
    [SerializeField] Tilemap wallMap;
    [SerializeField] Tilemap laserIOMap;
    [SerializeField] Tilemap mirrorMap;
    [SerializeField] Tilemap playerMap;

    private string levelFolder = "Assets/Levels/";
    [SerializeField] private string outgoingLevelName;
    [SerializeField] private string outgoingSceneName;

    void Start()
    {
        GenerateLevelLayout();

        SceneManager.LoadScene(outgoingSceneName);
    }

    private void GenerateLevelLayout()
    {
        string layout = "";

        Vector3Int levelDims = groundMap.size;
        int rightBound = levelDims.x / 2;
        int leftBound = -rightBound;
        int upBound = levelDims.y / 2;
        int downBound = -upBound;
        for (int y = upBound - 1; y >= downBound; y--)
        {
            for (int x = leftBound; x < rightBound; x++)
            {
                Vector3Int currentPos = new Vector3Int(x, y, 0);
                layout += GetParticipant(wallMap, currentPos);
                layout += GetParticipant(laserIOMap, currentPos);
                layout += GetParticipant(mirrorMap, currentPos);
                layout += GetParticipant(playerMap, currentPos);

                if (x < rightBound - 1)
                {
                    layout += GameGrid.GetDelim('C');
                }
            }

            if (y > downBound)
            {
                layout += GameGrid.GetDelim('R');
            }
        }

        WriteToFile(layout);
    }

    private void WriteToFile(string text)
    {
        string fullPath = levelFolder + outgoingLevelName + ".txt";

        //Writes the text to a text file for long-term storage.
        StreamWriter writer = File.CreateText(fullPath);
        writer.Write(text);
        writer.Close();

        //Writes the text to a TextAsset for short term storage, to "pass" to the gameplay scene.
        GameGrid.SetLayout(new TextAsset(text));
    }

    private string GetParticipant(Tilemap source, Vector3Int targetPos)
    {
        EditorTile thisTile = null;
        TileBase tempTile = source.GetTile(targetPos);
        if (tempTile != null)
        {
            thisTile = (EditorTile) tempTile;
        }

        if (thisTile != null)
        {
            return thisTile.GetTileCharData();
        }
        else
        {
            return "";
        }
    }
}
