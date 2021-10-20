using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    /** In order to queue board state updates, ask the grid to move an actor into an adjacent tile, etc.,
     * every Actor contains a reference to the gameGrid object responsible for grid logic in a level.*/
    protected GameGrid gameGrid;
    /** The cardinal direction this Actor is facing, as a 2D vector. */
    protected Vector2Int facing;
    /** The position on the game grid that this Actor occupies. Refers NOT to world position, 
     * but rather a column and row in the game grid's internal array. */
    protected Vector2Int gridPosition;

    /** The sprite that should be rendered for this Actor when it faces left. */
    [SerializeField] protected Sprite leftSprite;
    /** The sprite that should be rendered for this Actor when it faces right. */
    [SerializeField] protected Sprite rightSprite;
    /** The sprite that should be rendered for this Actor when it faces up. */
    [SerializeField] protected Sprite upSprite;
    /** The sprite that should be rendered for this Actor when it faces down. */
    [SerializeField] protected Sprite downSprite;

    /** Whether this Actor is able to move from square to square, including the player. */
    [SerializeField] protected bool isMovable;
    /** Whether this Actor will stop other Actors from moving into the square it occupies. */
    [SerializeField] protected bool isStop;

    private static float moveTime = 0.1f;
    private Queue<MoveCommand> moveBuffer = new Queue<MoveCommand>();

    private struct MoveCommand
    {
        public MoveCommand(Vector2Int moveOriginIn, Vector2Int moveDirectionIn)
        {
            moveOrigin = moveOriginIn;
            moveDirection = moveDirectionIn;
        }

        public Vector2Int moveOrigin { get; set; }
        public Vector2Int moveDirection { get; set; }
    }

    public virtual void Update()
    {
        if (moveBuffer.Count > 0)
        {
            Vector2Int moveOrigin = moveBuffer.Peek().moveOrigin;
            Vector2Int moveDirection = moveBuffer.Peek().moveDirection;
            Vector2 posOffset = new Vector2(moveDirection.x * (Time.deltaTime / moveTime), moveDirection.y * (Time.deltaTime / moveTime));
            gameObject.transform.position += new Vector3(posOffset.x, posOffset.y, 0);
            //gameObject.transform.position += new Vector3(0.25f, 0, 0);
            Vector2 goalPosition = new Vector2(moveOrigin.x + moveDirection.x, moveOrigin.y - moveDirection.y);

            if (moveDirection.Equals(Vector2Int.left))
            {
                if (gameObject.transform.position.x <= goalPosition.x)
                {
                    moveBuffer.Dequeue();
                    InstantActorPosUpdate(new Vector2Int((int) goalPosition.x, (int)goalPosition.y), true, false);
                }
            }
            else if (moveDirection.Equals(Vector2Int.right))
            {
                if (gameObject.transform.position.x >= goalPosition.x)
                {
                    moveBuffer.Dequeue();
                    InstantActorPosUpdate(new Vector2Int((int)goalPosition.x, (int)goalPosition.y), true, false);
                }
            }
            else if (moveDirection.Equals(Vector2Int.up))
            {
                if (gameObject.transform.position.y >= -goalPosition.y)
                {
                    moveBuffer.Dequeue();
                    InstantActorPosUpdate(new Vector2Int((int)goalPosition.x, (int)goalPosition.y), true, false);
                }
            }
            else if (moveDirection.Equals(Vector2Int.down))
            {
                if (gameObject.transform.position.y <= -goalPosition.y)
                {
                    moveBuffer.Dequeue();
                    InstantActorPosUpdate(new Vector2Int((int)goalPosition.x, (int)goalPosition.y), true, false);
                }
            }

            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(goalPosition.x - transform.position.x, -goalPosition.y - transform.position.y);
        }
    }

    /**
     * Sets the gameGrid reference for this Actor.
     * @param gameGrid - a reference to the Grid Controller object that handles grid logic for this level.
     */
    public void SetGameGrid(GameGrid gameGrid)
    {
        this.gameGrid = gameGrid;
    }

    /**
     * Gets the 2D vector that represents which direction this Actor is facing.
     * @returns this Actor's facing vector.
     */
    public Vector2Int GetFacing()
    {
        return facing;
    }

    /**
     * Sets the 2D vector that represents which direction this Actor is facing.
     * Also responsible for setting the initial sprite for this Actor, based on which direction is given.
     * @param dirId - a character value (for Actors not overriding this method this will only consist of 'L', 'R', 'U', or 'D') corresponding to a direction.
     */
    public virtual void SetFacing(char dirId)
    {
        switch (dirId)
        {
            case 'L':
                {
                    facing = Vector2Int.left;
                    gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                    break;
                }
            case 'R':
                {
                    facing = Vector2Int.right;
                    gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
                    break;
                }
            case 'U':
                {
                    facing = Vector2Int.up;
                    gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
                    break;
                }
            case 'D':
                {
                    facing = Vector2Int.down;
                    gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
                    break;
                }
        }
    }

    /**
     * Gets the position of the grid square this Actor occupies.
     * @return a 2D vector corresponding to an index in the game grid's internal array at which this Actor is stored.
     */
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    /**
     * Sets the position of the grid square this Actor occupies.
     * @param gridPosition - the 2D vector corresponding to the index in the game grid's internal array at which the Actor is now stored.
     */
    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    /**
     * Returns whether or not this Actor is able to move from square to square in the game grid.
     * @return a boolean value representing the movability of this Actor.
     */
    public bool GetIsMovable()
    {
        return isMovable;
    }

    /**
     * Returns whether or not this Actor will stop other Actors from moving into its square on the grid.
     * @return a boolean value representing the stoppability(?) of this Actor.
     */
    public bool GetIsStop()
    {
        return isStop;
    }

    /**
     * Given a new position in the grid, update both this actor's grid position and the
     * underlying GameObject's transform position accordingly.
     */
    public void InstantActorPosUpdate(Vector2Int newPos, bool updateTransformAndCollider, bool updateGridPosition)
    {
        Debug.Log(gridPosition);
        
        //"Grid positions" refer to a column/row of the game grid array, where the top row is labelled "0".
        //This is why we flip the y coordinate; to flip the final image vertically
        if (updateGridPosition)
        {
            gridPosition = newPos;
        }

        if (updateTransformAndCollider)
        {
            gameObject.transform.position = new Vector3(newPos.x, -newPos.y);
        }

        if (GetComponent<BoxCollider2D>() != null && moveBuffer.Count == 0)
        {
            GetComponent<BoxCollider2D>().offset = Vector2.zero;
        }
    }

    public void SlowActorPosUpdate(Vector2Int startPos, Vector2Int moveDir)
    {
        MoveCommand thisCommand = new MoveCommand(startPos, moveDir);
        moveBuffer.Enqueue(thisCommand);
    }

    public virtual void SelfDestruct()
    {
        Destroy(gameObject);
    }

    public virtual void PerformButtonAction()
    {

    }
}
