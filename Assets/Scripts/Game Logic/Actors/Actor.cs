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
    public void UpdateActorPos(Vector2Int newPos)
    {
        gridPosition = newPos;
        //"Grid positions" refer to a column/row of the game grid array, where the top row is labelled "0".
        //This is why we flip the y coordinate; to flip the final image vertically
        gameObject.transform.position = new Vector3(gridPosition.x, -gridPosition.y);
    }

    public virtual void SelfDestruct()
    {
        Destroy(gameObject);
    }

    public virtual void PerformButtonAction()
    {

    }
}
