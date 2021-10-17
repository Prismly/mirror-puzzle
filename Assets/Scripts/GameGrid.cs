using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    //(0, 0) is in the BOTTOM-LEFT corner

    [SerializeField]
    private TextAsset levelLayout;
    private char rowDelim = ':';
    private char colDelim = '-';

    private GridSquare[,] gridArray;

    [SerializeField]
    private GameObject groundPrefab;
    private GameObject groundContainer;

    [SerializeField]
    private GameObject wallPrefab;
    private GameObject wallContainer;

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
        GenerateLevelObjects();
    }

    private void GenerateLevelObjects()
    {
        groundContainer = new GameObject("Ground Tiles");
        wallContainer = new GameObject("Wall Tiles");

        //First row is processed separately in order to initialize gridArray with the corrent amount of columns.
        string[] layoutByRow = levelLayout.text.Split(':');
        string[] layoutByCol = layoutByRow[0].Split('-');
        gridArray = new GridSquare[layoutByRow.Length, layoutByCol.Length];
        for (int x = 0; x < gridArray.GetLength(1); x++)
        {
            ParseTileString(layoutByCol[x], new Vector2Int(x, 0));
        }

        for (int y = 1; y < gridArray.GetLength(0); y++)
        {
            layoutByCol = layoutByRow[y].Split('-');

            for (int x = 0; x < gridArray.GetLength(1); x++)
            {
                ParseTileString(layoutByCol[x], new Vector2Int(x, y));
            }
        }
    }

    private void ParseTileString(string tileString, Vector2Int pos)
    {
        char[] thisTile = tileString.ToCharArray();
        List<char> thisTilePrefabIds = new List<char>();
        List<char> thisTileDirIds = new List<char>();
        for (int i = 0; i < thisTile.Length; i += 2)
        {
            thisTilePrefabIds.Add(thisTile[i]);
            thisTileDirIds.Add(thisTile[i + 1]);
        }

        CreateGridSquare(pos, thisTilePrefabIds, thisTileDirIds);
    }

    /**
     *  Constructs a GridSquare object, then assigns it to its position in the gridArray.
     *  The resulting GridSquare object contains a list of the Actors which occupy it.
     */
    private void CreateGridSquare(Vector2Int pos, List<char> prefabIds, List<char> dirIds)
    {
        List<GameObject> occupants = new List<GameObject>();

        for(int i = 0; i < prefabIds.Count; i++)
        {
            GameObject actorPrefab = ActorLibrary.GetPrefab(prefabIds[i]);
            
            if (actorPrefab != null)
            {
                GameObject actor = Instantiate(actorPrefab);
                Actor actorController = actor.GetComponent<Actor>();
                actorController.SetFacing((Cardinal) dirIds[i]);
                actorController.SetGridPosition(pos);
                actorController.SetGameGrid(this);

                //Sets this actor as the child of a gameobject called "Wall Tiles" if it was created using the wall prefab.
                //This is exclusively to make the object hierarchy more readable while testing.
                if(actorPrefab == wallPrefab)
                {
                    actor.transform.parent = wallContainer.transform;
                }

                occupants.Add(actor);
            }
        }

        GameObject ground = Instantiate(groundPrefab);
        ground.transform.parent = groundContainer.transform;
        occupants.Add(ground);

        gridArray[pos.y, pos.x] = new GridSquare(pos, occupants);
    }

    /**
     *  Represents a single in-game tile, which knows its position on the grid, and contains
     *  an Actor GameObject, among other things.
     */
    private class GridSquare
    {
        private Vector2Int gridPosition;
        /** Container exists to group all objects on this tile together, so they can be moved/transformed in unison */
        private List<GameObject> occupants = new List<GameObject>();

        //TODO: Change 'occupant' to 'occupants' and make it a list? Less efficient but will allow for many layers including blocks, laser I/O, lasers (maybe) and buttons
        public GridSquare(Vector2Int gridPositionIn, List<GameObject> occupantsIn)
        {
            gridPosition = gridPositionIn;

            foreach(GameObject o in occupantsIn)
            {
                occupants.Add(o);
            }

            for(int i = 0; i < occupants.Count; i++)
            {
                occupants[i].transform.position = new Vector3(gridPosition.x, -gridPosition.y, 0);
            }
        }

        public GameObject GetOccupantAt(int index)
        {
            return occupants[index];
        }

        public int GetFirstMovableOccupantIndex()
        {
            for(int i = 0; i < occupants.Count; i++)
            {
                if(occupants[i].GetComponent<Actor>().GetIsMovable())
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetFirstSolidOccupantIndex()
        {
            for(int i = 0; i < occupants.Count; i++)
            {
                if (occupants[i].GetComponent<Actor>().GetIsSolid())
                {
                    return i;
                }
            }
            return -1;
        }

        public void AddOccupant(GameObject newOccupant)
        {
            occupants.Add(newOccupant);
        }

        public void GiveOccupantTo(GridSquare squareToGiveTo)
        {
            //Assumes that a movable occupant exists in the caller square, this will have already been checked.
            int movingActorIndex = GetFirstMovableOccupantIndex();
            GameObject movingActor = occupants[movingActorIndex];
            squareToGiveTo.AddOccupant(movingActor);
            occupants.RemoveAt(movingActorIndex);
            movingActor.GetComponent<Actor>().UpdateActorPos(squareToGiveTo.gridPosition);
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
        if(startSquare.GetFirstMovableOccupantIndex() != -1)
        {
            if(endSquare.GetFirstMovableOccupantIndex() == -1)
            {
                //There is nothing movable in the way, check if there is anything solid...
                if(endSquare.GetFirstSolidOccupantIndex() == -1)
                {
                    //There is nothing obstructive in the way, so the actor is free to move into that spot.
                    startSquare.GiveOccupantTo(endSquare);
                    return true;
                }
            }
            else if(endSquare.GetOccupantAt(endSquare.GetFirstMovableOccupantIndex()).GetComponent<Actor>().GetIsMovable() && !pushing)
            {
                //There is something in the way, if it is pushable and isn't already being pushed by a non-player, perform all these checks again on the new position.
                if(MoveActorInGrid(endPos, dir, true))
                {
                    //We successfully pushed the object in front of us, thus we are clear to move too
                    startSquare.GiveOccupantTo(endSquare);
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
