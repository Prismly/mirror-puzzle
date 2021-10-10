using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    //(0, 0) is in the BOTTOM-LEFT corner

    [SerializeField]
    private TextAsset levelLayout;

    private GridSquare[,] gridArray;

    [SerializeField]
    private GameObject gridSquarePrefab;

    public enum Cardinal
    {
        LEFT = 'L',
        RIGHT = 'R',
        UP = 'U',
        DOWN = 'D'
    }

    public void Start()
    {
        //First row is processed separately in order to initialize gridArray with the corrent amount of columns.
        string[] layoutByRow = levelLayout.text.Split('\n');
        string[] layoutByCol = layoutByRow[0].Split('-');
        gridArray = new GridSquare[layoutByRow.Length, layoutByCol.Length];
        for (int x = 0; x < gridArray.GetLength(1); x++)
        {
            char[] thisTile = layoutByCol[x].ToCharArray();
            CreateGridSquare(new Vector2Int(x, 0), thisTile[0], thisTile[1]);
        }

        for (int y = 1; y < gridArray.GetLength(0); y++)
        {
            layoutByCol = layoutByRow[y].Split('-');

            for (int x = 0; x < gridArray.GetLength(1); x++)
            {
                char[] thisTile = layoutByCol[x].ToCharArray();
                CreateGridSquare(new Vector2Int(x, y), thisTile[0], thisTile[1]);
            }
        }
    }

    private void CreateGridSquare(Vector2Int pos, char prefabId, char dirId)
    {
        GameObject actorPrefab = ActorLibrary.GetPrefab(prefabId);
        GameObject actor = null;

        if (actorPrefab != null)
        {
            actor = Instantiate(actorPrefab);
            Actor actorController = actor.GetComponent<Actor>();
            actorController.SetGameGrid(this);
            actorController.SetFacing((Cardinal)dirId);
            actorController.SetGridPosition(pos);
        }

        gridArray[pos.y, pos.x] = new GridSquare(gridSquarePrefab, pos, actor);
    }

    private class GridSquare
    {
        private Vector2Int gridPosition;
        private GameObject container;
        private GameObject occupant;

        public GridSquare(GameObject gridSquarePrefab, Vector2Int gridPositionIn, GameObject occupantIn)
        {
            gridPosition = gridPositionIn;
            container = Instantiate(gridSquarePrefab);
            container.name += "(" + gridPosition.x + ", " + gridPosition.y + ")";
            occupant = occupantIn;

            if(occupant != null)
            {
                occupant.transform.parent = container.transform;
            }

            container.transform.position = new Vector3(gridPosition.x, -gridPosition.y, 0);
        }
    }

    public bool MoveActor(Vector2Int targetPos, Cardinal dir)
    {
        Debug.Log(dir);
        return false;
    }
}
