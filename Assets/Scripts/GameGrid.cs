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

        public GameObject GetOccupant()
        {
            return occupant;
        }

        public void SwapOccupants(GridSquare squareToSwapWith)
        {
            GameObject temp = squareToSwapWith.occupant;
            squareToSwapWith.occupant = occupant;
            if(squareToSwapWith.occupant != null)
            {
                squareToSwapWith.occupant.GetComponent<Actor>().SetGridPosition(squareToSwapWith.gridPosition);
                squareToSwapWith.occupant.transform.parent = squareToSwapWith.container.transform;
                squareToSwapWith.occupant.transform.position = new Vector3(squareToSwapWith.gridPosition.y, squareToSwapWith.gridPosition.x);
            }
            occupant = temp;
            if(occupant != null)
            {
                occupant.GetComponent<Actor>().SetGridPosition(gridPosition);
                occupant.transform.parent = container.transform;
                occupant.transform.position = new Vector3(gridPosition.x, -gridPosition.y);
            }
        }
    }

    /**
     *  Attempt to move the actor at the given position in the given direction.
     *  Handles logic related to pushing another object and hitting a non-moveable object.
     *  @param targetPos - the grid coordinates of the actor to move
     *  @param dir - the direction in which to move the actor
     *  @return whether the actor was able to move into the next space.
     */
    public bool MoveActorInGrid(Vector2Int startPos, Cardinal dir, bool pushing)
    {
        Vector2Int endPos = startPos + CardinalToTransform(dir);
        GridSquare startSquare = GetGridSquare(startPos);
        GridSquare endSquare = GetGridSquare(endPos);
        
        //If there is an actor in the starting position to move...
        if(startSquare.GetOccupant() != null)
        {
            if(endSquare.GetOccupant() == null)
            {
                //There is nothing in the way, this actor is free to move into that spot.
                endSquare.SwapOccupants(startSquare);
                return true;
            }
            else if(endSquare.GetOccupant().GetComponent<Actor>().GetIsMoveable() && !pushing)
            {
                //There is something in the way, if it is pushable and isn't already being pushed by a non-player, perform all these checks again on the new position.
                if(MoveActorInGrid(endPos, dir, true))
                {
                    //We successfully pushed the object in front of us, thus we are clear to move too
                    endSquare.SwapOccupants(startSquare);
                    return true;
                }
            }
        }

        return false;
    }

    /**
     *  Returns the GridSquare occupying the tile at the specified location in the grid.
     *  Returns null if requesting a square outside the bounds of the gridArray.
     */
    private GridSquare GetGridSquare(Vector2Int targetPos)
    {
        //If the given position is within the bounds of the gridArray...
        if(targetPos.x < gridArray.GetLength(1) && targetPos.x >= 0 &&
            targetPos.y < gridArray.GetLength(0) && targetPos.y >= 0)
        {
            return gridArray[targetPos.y, targetPos.x];
        }
        return null;
    }

    /**
     *  Converts a Cardinal direction enum value into a Vector2Int with the corresponding directions.
     *  e.g. Cardinal.LEFT results in Vector2Int(-1, 0) because it points one unit LEFT of the origin.
     */
    private Vector2Int CardinalToTransform(Cardinal dir)
    {
        switch (dir)
        {
            case Cardinal.LEFT:
                {
                    return new Vector2Int(-1, 0);
                }
            case Cardinal.RIGHT:
                {
                    return new Vector2Int(1, 0);
                }
            case Cardinal.UP:
                {
                    //Flipped because GameObjects are drawn with position (x, -y)
                    return new Vector2Int(0, -1);
                }
            case Cardinal.DOWN:
                {
                    //Flipped because GameObjects are drawn with position (x, -y)
                    return new Vector2Int(0, 1);
                }
            default:
                {
                    Debug.LogError("Invalid Cardinal direction was passed to CardinalToTransform.");
                    return new Vector2Int(int.MaxValue, int.MaxValue);
                }
        }
    }
}
