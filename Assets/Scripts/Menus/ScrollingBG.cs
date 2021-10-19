using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollingBG : MonoBehaviour
{
    private static int bottomLeftBound = -10;
    private static int topRightBound = 10;
    [SerializeField] private int bottomLeftStart;
    [SerializeField] private int topRightStart;
    private static float incrementVal = 0.001f;
    private Vector3 increment;

    [SerializeField] Tile[] possibleTiles;

    private Tilemap mapComponent;

    private void Start()
    {
        increment = new Vector3(incrementVal, incrementVal, 0);
        gameObject.transform.position = new Vector3Int(bottomLeftStart, bottomLeftStart, 0);
        mapComponent = gameObject.GetComponent<Tilemap>();
        ShuffleTiles();
    }

    void Update()
    {
        gameObject.transform.position += increment;
        if(gameObject.transform.position.x > topRightBound && gameObject.transform.position.y > topRightBound)
        {
            ShuffleTiles();
            gameObject.transform.position = new Vector3Int(bottomLeftBound, bottomLeftBound, 0);
        }
    }

    private void ShuffleTiles()
    {
        for(int x = -19; x < 19; x++)
        {
            for(int y = -5; y < 5; y++)
            {
                int minIndex = 0;
                int maxIndex = possibleTiles.Length - 1;
                int tileIndex = Random.Range(minIndex, maxIndex);
                mapComponent.SetTile(new Vector3Int(x, y, 0), possibleTiles[tileIndex]);
            }
        }
    }
}
