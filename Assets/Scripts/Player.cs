using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    /** The keyboard key corresponding to the command to move the player character LEFT by one square. */
    private KeyCode leftMovement = KeyCode.LeftArrow;
    /** The keyboard key corresponding to the command to move the player character RIGHT by one square. */
    private KeyCode rightMovement = KeyCode.RightArrow;
    /** The keyboard key corresponding to the command to move the player character UP by one square. */
    private KeyCode upMovement = KeyCode.UpArrow;
    /** The keyboard key corresponding to the command to move the player character DOWN by one square. */
    private KeyCode downMovement = KeyCode.DownArrow;

    /**
     * Called once every frame, runs the function that detects player input.
     */
    public void Update()
    {
        playerInput();
    }

    /**
     * Detects when the player presses a movement key, faces the player object in the direction pressed,
     * attempts to move the player in the direction pressed (although this will do nothing if blocked by a wall or similar),
     * as well as redrawing all lasers on the screen, since they may bounce differently after the move.
     */
    private void playerInput()
    {
        if (Input.GetKeyDown(leftMovement))
        {
            //LEFT MOVEMENT
            gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
            base.SetFacing('L');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.left, false);
            gameGrid.QueueLaserIOUpdate();
        }
        if (Input.GetKeyDown(rightMovement))
        {
            //RIGHT MOVEMENT
            gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
            base.SetFacing('R');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.right, false);
            gameGrid.QueueLaserIOUpdate();
        }
        if (Input.GetKeyDown(upMovement))
        {
            //UPWARD MOVEMENT
            gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
            base.SetFacing('U');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.up, false);
            gameGrid.QueueLaserIOUpdate();
        }
        if (Input.GetKeyDown(downMovement))
        {
            //DOWNWARD MOVEMENT
            gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
            base.SetFacing('D');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.down, false);
            gameGrid.QueueLaserIOUpdate();
        }
    }
}