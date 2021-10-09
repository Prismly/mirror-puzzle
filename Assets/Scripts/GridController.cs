using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    //(0, 0) is in the BOTTOM-LEFT corner

    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    private const int groundPrefabId = 0;
    private const int wallPrefabId = 1;
    private const int playerPrefabId = 2;

    [SerializeField]
    GameObject prefabLibrary;

    private GridSquare[,] gridArray;

    public void Start()
    {
        gridArray = new GridSquare[height, width];

        //Presumably this is where an external level layout would be read in and used to initialize the gridArray
        //For now I'll hard-code it
        for (int x = 0; x < gridArray.GetLength(1); x++)
        {
            for(int y = 0; y < gridArray.GetLength(0); y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                GameObject actor = null;

                if (x == 0 || x == gridArray.GetLength(1) - 1 || y == 0 || y == gridArray.GetLength(0) - 1)
                {
                    actor = Instantiate(ActorTileLibrary.GetInstance().GetPrefab(wallPrefabId));
                }
                
                gridArray[y, x] = new GridSquare(pos, actor);
            }
        }
    }

    private class GridSquare
    {
        private Vector2Int gridPosition;
        private GameObject container;
        private GameObject groundTile;
        private GameObject occupant;

        public GridSquare(Vector2Int gridPositionIn, GameObject occupantIn)
        {
            gridPosition = gridPositionIn;
            container = new GameObject();
            occupant = occupantIn;
            groundTile = Instantiate(ActorTileLibrary.GetInstance().GetPrefab(groundPrefabId));

            groundTile.transform.parent = container.transform;
            if(occupant != null)
            {
                occupant.transform.parent = container.transform;
            }

            container.transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        }
    }
}
