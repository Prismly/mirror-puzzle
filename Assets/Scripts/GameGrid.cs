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

    /**
     *  Refers to each of the cardinal directions, for use when directing Actors to move on the grid,
     *  as well as specifying which way each faces.
     */
    public enum Cardinal
    {
        LEFT = 'L',
        RIGHT = 'R',
        UP = 'U',
        DOWN = 'D'
    }

    /**
     *  Runs on scene startup, responsible for initializing the gridArray and populating it
     *  based on the levelLayout text file resource attached to this GameGrid GameObject.
     */
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

    /**
     *  Constructs a GridSquare object, then assigns it to its position in the gridArray.
     *  The resulting GridSquare object contains the given Actor GameObject, if one was given,
     *  as well as facing the direction given by dirId.
     */
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

    /**
     *  Represents a single in-game tile, which knows its position on the grid, and contains
     *  an Actor GameObject, among other things.
     */
    private class GridSquare
    {
        private Vector2Int gridPosition;
        /** Container exists to group all objects on this tile together, so they can be moved/transformed in unison */
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

    /**
     *  Attempt to move the actor at the given position in the given direction.
     *  Handles logic related to pushing another object and hitting a non-moveable object.
     *  @param targetPos - the grid coordinates of the actor to move
     *  @param dir - the direction in which to move the actor
     *  @return whether the actor was able to move into the next space.
     */
    public bool MoveActor(Vector2Int targetPos, Cardinal dir)
    {
        Debug.Log(dir);
        return false;
    }
}
