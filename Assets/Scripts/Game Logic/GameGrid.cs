using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    /** The text file to read from when instantiating the grid and its objects. 
     * Contains all the information to build a level from prefabs. */
    private static TextAsset levelLayout;
    [SerializeField] private TextAsset defaultLevelLayout;
    /** The delimiter to split rows up by in the levelLayout text file. */
    private static char rowDelim = ':';
    /** The delimiter to split columns up by in the levelLayout text file. */
    private static char colDelim = '-';

    /** The internal array that stores groups of Actors, the fundamental objects 
     * that do things in the game world. The actual data type is GridSquare, a private 
     * inner class that contains positional information as well as the occupying Actors. */
    private GridSquare[,] gridArray;
    private bool levelIsActive;

    /** The prefab for a ground tile GameObject. One is generated on every GridSquare, 
     * so that even if things move, there's always at least terrain. */
    [SerializeField] private GameObject groundPrefab;

    /** Empty GameObjects that serve as containers to store all the Ground/Wall Actors in, 
     * making the hierarchy look less cluttered at runtime. */
    private GameObject groundContainer;
    private GameObject wallContainer;

    /** A flag that allows the very first laser update to occur one physics frame after all 
     * objects are created, which is necessary because of a Unity quirk. */
    private bool startupStallFrame = true;
    /** A flag that triggers all lasers on the board to update their paths on the next physics frame. */
    private bool boardStateChanged = false;

    /** A list of every laser output tile in the level, used for updating lasers from all sources on the grid. */
    private List<GameObject> laserOuts = new List<GameObject>();
    /** A list of every laser input tile in the level, used both to turn them off before every laser update 
     * and to check if they are all lit, in which case the level is complete. */
    private List<GameObject> laserIns = new List<GameObject>();

    /** The factor by which to reduce each dimension of an Actor's BoxCollider2D, to reduce the amount of edge-to-edge collisions. */
    private float colliderReductionOffset = 0.1f;

    [SerializeField] Camera sceneCamera;

    /**
     * Runs on scene startup, responsible for initializing the gridArray and populating it
     * based on the levelLayout text file resource attached to this GameGrid GameObject.
     */
    public void Start()
    {
        if(levelLayout == null)
        {
            levelLayout = defaultLevelLayout;
        }
        GenerateLevelObjects();
        levelIsActive = true;

        sceneCamera.transform.position = new Vector3(((float)gridArray.GetLength(1) / 2) - 0.5f, -((float) gridArray.GetLength(0) / 2) + 0.5f, -10);
        sceneCamera.orthographicSize = gridArray.GetLength(0) * 0.5f;
    }

    /**
     * Runs every physics frame, updates laser paths if a board state change recently occurred.
     */
    public void FixedUpdate()
    {
        if(startupStallFrame)
        {
            //Stall for a single frame before executing the IO update function.
            //This is because colliders don't exist until the frame AFTER all the objects are generated. It's weird..
            startupStallFrame = false;
            boardStateChanged = true;
        }
        else if(boardStateChanged)
        {
            Physics2D.SyncTransforms();
            LaserIOUpdate();
            //We don't need to call LaserIOUpdate() every frame, only when the board state changes.
            boardStateChanged = false;
        }
    }

    /**
     * Initializes the gridArray array and cuts up the input levelLayout text file, calling
     * ParseTileString on each individual square.
     */
    private void GenerateLevelObjects()
    {
        groundContainer = new GameObject("Ground Tiles");
        wallContainer = new GameObject("Wall Tiles");

        //First row is processed separately in order to initialize gridArray with the corrent amount of columns.
        string[] layoutByRow = levelLayout.text.Split(rowDelim);
        string[] layoutByCol = layoutByRow[0].Split(colDelim);
        gridArray = new GridSquare[layoutByRow.Length, layoutByCol.Length];
        for (int x = 0; x < gridArray.GetLength(1); x++)
        {
            ParseTileString(layoutByCol[x], new Vector2Int(x, 0));
        }

        for (int y = 1; y < gridArray.GetLength(0); y++)
        {
            layoutByCol = layoutByRow[y].Split(colDelim);

            for (int x = 0; x < gridArray.GetLength(1); x++)
            {
                ParseTileString(layoutByCol[x], new Vector2Int(x, y));
            }
        }
    }

    /**
     * Separates out pairs of Actor character Ids from the given string, and delegates the job of creating a GridSquare object
     * out of them at the given position to the CreateGridSquare function.
     * @param tileString - a string of characters that can be parsed into Actor-Direction pairs, e.g. 'WU' for "Wall, Up".
     * @param pos - the grid position of the intended grid square.
     */
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
     * Constructs a GridSquare object, then assigns it to its position in the gridArray.
     * The resulting GridSquare object contains a list of the Actors which occupy it.
     * @param pos - the grid position of the new GridSquare.
     * @param prefabIds - a list of character values, which refer to prefab instances.
     * @param dirIds - a list of character values, which refer to the starting facing direction of the corresponding prefab instance in prefabIds.
     */
    private void CreateGridSquare(Vector2Int pos, List<char> prefabIds, List<char> dirIds)
    {
        List<GameObject> occupants = new List<GameObject>();

        for(int i = 0; i < prefabIds.Count; i++)
        {
            GameObject actorPrefab = ActorLibrary.GetPrefab(prefabIds[i]);
            
            if(actorPrefab != null)
            {
                //Insantiate a new instance of this actor, and initialize some of its fields.
                GameObject actor = Instantiate(actorPrefab);
                Actor actorController = actor.GetComponent<Actor>();
                actorController.SetFacing(dirIds[i]);
                actorController.SetGameGrid(this);

                //Reduces the default size of the new actor's box collider by a constant factor to avoid collisions when two objects are adjacent.
                actor.GetComponent<BoxCollider2D>().size = new Vector2(actor.GetComponent<BoxCollider2D>().size.x - colliderReductionOffset, 
                    actor.GetComponent<BoxCollider2D>().size.y - colliderReductionOffset);

                //Perform actor-specific actions, such as adding a new Wall actor to the Wall container, or adding a LaserIO actor to its corresponding list.
                if(actorPrefab.GetComponent<Wall>() != null)
                {
                    actor.transform.parent = wallContainer.transform;
                }
                else if(actorPrefab.GetComponent<LaserOut>() != null)
                {
                    laserOuts.Add(actor);
                }
                else if (actorPrefab.GetComponent<LaserIn>() != null)
                {
                    laserIns.Add(actor);
                }

                occupants.Add(actor);
            }
        }

        //A ground Actor is generated for every GridSquare.
        GameObject ground = Instantiate(groundPrefab);
        ground.transform.parent = groundContainer.transform;
        occupants.Add(ground);

        gridArray[pos.y, pos.x] = new GridSquare(pos, occupants);
    }

    /**
     *  Represents a single in-game tile, which knows its position on the grid, and contains
     *  one or more Actor GameObjects, among other things.
     */
    private class GridSquare
    {
        /** The position of this GridSquare in the game grid array. */
        private Vector2Int gridPosition;
        /** The list of Actor objects currently occupying this GridSquare. */
        private List<GameObject> occupants = new List<GameObject>();

        /**
         * Constructs a GridSquare with the given grid position and actor occupants, 
         * and sets the position of all the newly assigned Actors accordingly.
         * @param gridPositionIn - the grid array position at which this GridSquare will reside.
         * @param occupantsIn - a list of Actor GameObjects, all of which occupy the new GridSquare.
         */
        public GridSquare(Vector2Int gridPositionIn, List<GameObject> occupantsIn)
        {
            gridPosition = gridPositionIn;

            foreach(GameObject o in occupantsIn)
            {
                occupants.Add(o);
            }

            for(int i = 0; i < occupants.Count; i++)
            {
                occupants[i].GetComponent<Actor>().UpdateActorPos(gridPositionIn);
            }
        }

        /**
         * Returns the Actor GameObject located at the given index in the occupants list.
         * @param index - the index of the Actor GameObject to return.
         * @returns the Actor GameObject at the specified index.
         */
        public GameObject GetOccupantAt(int index)
        {
            return occupants[index];
        }

        /**
         * Instead of returning just one occupant, like GetOccupantAt, returns the entire list of occupants
         * for perusal by the caller.
         * @return all occupants of this GridSquare, in a list.
         */
        public List<GameObject> GetOccupants()
        {
            return occupants;
        }

        /**
         * Returns the first occupant in the occupants array to have the isMovable flag set to true.
         * @returns the index of the first movable occupant in the list, or -1 if no such occupant exists.
         */
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

        /**
         * Returns the first occupant in the occupants array to have the isStop flag set to true.
         * @returns the index of the first stopping occupant in the list, or -1 if no such occupant exists.
         */
        public int getFirstStoppingOccupantIndex()
        {
            for(int i = 0; i < occupants.Count; i++)
            {
                if (occupants[i].GetComponent<Actor>().GetIsStop())
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Adds a new occupant to the occupants list for this GridSquare.
         * @param newOccupant - the new occupant to add to the list.
         */
        public void AddOccupant(GameObject newOccupant)
        {
            occupants.Add(newOccupant);
        }

        /**
         * Takes the first movable occupant of the caller GridSquare and moves it from its own occupants list
         * to that of the specified adjacent GridSquare. GiveOccupantTo assumes that the caller contains a
         * movable occupant; behavior is undefined otherwise.
         * @param squareToGiveTo - the GridSquare to give the moving actor.
         */
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
    public bool MoveActorInGrid(Vector2Int startPos, Vector2Int dir, bool pushing)
    {
        Vector2Int dirOffset = dir;
        dirOffset.y = -dirOffset.y;
        Vector2Int endPos = startPos + dirOffset;
        GridSquare startSquare = GetGridSquare(startPos);
        GridSquare endSquare = GetGridSquare(endPos);
        
        //If there is an actor in the starting position to move...
        if(startSquare.GetFirstMovableOccupantIndex() != -1)
        {
            if(endSquare.GetFirstMovableOccupantIndex() == -1)
            {
                //There is nothing movable in the way, check if there is anything solid...
                if(endSquare.getFirstStoppingOccupantIndex() == -1)
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
     * Returns the GridSquare occupying the tile at the specified location in the grid.
     * Returns null if requesting a square outside the bounds of the gridArray.
     * @param targetPos - the grid position of the square to retrieve.
     * @return the GridSquare at the given grid position.
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
     * Retrieves all Actors currently occupying the GridSquare at the given grid position.
     * @param targetPos - the grid position of the square to retrieve the Actors of.
     * @return a list of the Actors occupying the GridSquare at the given grid position.
     */
    public List<GameObject> GetGridSquareOccupants(Vector2Int targetPos)
    {
        return GetGridSquare(targetPos).GetOccupants();
    }

    /**
     * Gets the factor by which to reduce each dimension of an Actor's BoxCollider2D.
     * @return the collider reduction offset value.
     */
    public float GetColliderReductionOffset()
    {
        return colliderReductionOffset;
    }

    /**
     * Sets the flag that will cause a LaserIOUpdate next physics frame.
     */
    public void QueueLaserIOUpdate()
    {
        boardStateChanged = true;
    }

    /**
     * Updates the paths that lasers take through the level, called the frame atfer the board state changes for any reason.
     * Additionally, calls CheckForWin to determine if the new state is a winning one.
     */
    private void LaserIOUpdate()
    {
        //Laser ins are reset to unlit...
        UpdateLaserIns();
        //Laser outs fire lasers and, if they still point to the laser ins, those revert to lit...
        UpdateLaserOuts();
        //Check if, as a result if this most recent move, the board is in a winning state...
        CheckForWin();
    }

    /**
     * Re-fires a laser from all laser output actors, to trace a new path through the changed grid.
     */
    public void UpdateLaserOuts()
    {
        foreach (GameObject o in laserOuts)
        {
            o.GetComponent<LaserOut>().UpdateLasers();
        }
    }

    /**
     * Turns off all laser input actors before the laser output actors can re-fire their lasers.
     * This way, the state of a laser input actor is always synced with whether there is a laser firing into it.
     */
    public void UpdateLaserIns()
    {
        //Reset all ins for the next turn.
        foreach (GameObject o in laserIns)
        {
            o.GetComponent<LaserIn>().SetIsLit(false);
        }
    }

    /**
     * Checks if all laser input actors in the level are turned on simultaneously. If they are, the level is complete!
     */
    private void CheckForWin()
    {
        int litGoals = 0;
        foreach (GameObject o in laserIns)
        {
            if (o.GetComponent<LaserIn>().GetIsLit())
            {
                litGoals++;
            }
        }
        if (litGoals == laserIns.Count)
        {
            //All goals are lit, the player has finished the level!
            CompleteLevel();
        }
    }

    /**
     * Logic that should occur when the level is completed. As of yet unused.
     */
    private void CompleteLevel()
    {
        Debug.Log("Level Complete!");
    }

    public static char GetDelim(char specifier)
    {
        switch(specifier)
        {
            case 'R':
                {
                    return rowDelim;
                }
            case 'C':
                {
                    return colDelim;
                }
            default:
                {
                    Debug.LogError("You tried to get a delimiter that does not exist. Pass GetDelim 'R' for rows and 'C' for columns.");
                    return '\0';
                }
        }
    }

    public static void SetLayout(TextAsset layout)
    {
        levelLayout = layout;
    }

    public bool GetLevelIsActive()
    {
        return levelIsActive;
    }

    public void SetLevelIsActive(bool levelIsActiveIn)
    {
        levelIsActive = levelIsActiveIn;
    }

    public Vector2Int GetGridArrayDims()
    {
        return new Vector2Int(gridArray.GetLength(1), gridArray.GetLength(0));
    }
}
