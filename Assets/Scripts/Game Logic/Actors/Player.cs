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
    private KeyCode undo = KeyCode.Z;

    /** The z coordinate tracks the total number of actors moved by the action. */
    private Stack<Vector3Int> moves = new Stack<Vector3Int>();

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
        if(gameGrid.GetLevelIsActive())
        {
            if (Input.GetKeyDown(leftMovement))
            {
                //LEFT MOVEMENT
                gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                base.SetFacing('L');
                int movedActorCount = gameGrid.MoveActorInGrid(gridPosition, Vector2Int.left, false);
                if (movedActorCount > 0)
                {
                    moves.Push(new Vector3Int(-1, 0, movedActorCount));
                }
                gameGrid.QueueLaserIOUpdate();

            }
            if (Input.GetKeyDown(rightMovement))
            {
                //RIGHT MOVEMENT
                gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
                base.SetFacing('R');
                int movedActorCount = gameGrid.MoveActorInGrid(gridPosition, Vector2Int.right, false);
                if (movedActorCount > 0)
                {
                    moves.Push(new Vector3Int(1, 0, movedActorCount));
                }
                gameGrid.QueueLaserIOUpdate();
            }
            if (Input.GetKeyDown(upMovement))
            {
                //UPWARD MOVEMENT
                gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
                base.SetFacing('U');
                int movedActorCount = gameGrid.MoveActorInGrid(gridPosition, Vector2Int.up, false);
                if (movedActorCount > 0)
                {
                    moves.Push(new Vector3Int(0, 1, movedActorCount));
                }
                gameGrid.QueueLaserIOUpdate();
            }
            if (Input.GetKeyDown(downMovement))
            {
                //DOWNWARD MOVEMENT
                gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
                base.SetFacing('D');
                int movedActorCount = gameGrid.MoveActorInGrid(gridPosition, Vector2Int.down, false);
                if (movedActorCount > 0)
                {
                    moves.Push(new Vector3Int(0, -1, movedActorCount));
                }
                gameGrid.QueueLaserIOUpdate();
            }
            if (Input.GetKeyDown(undo))
            {
                //UNDO MOVEMENT
                if(moves.Count > 0)
                {
                    Vector3Int previousMove = moves.Pop();
                    
                    if(previousMove.x > 0)
                    {
                        base.SetFacing('R');
                    }
                    else if(previousMove.x < 0)
                    {
                        base.SetFacing('L');
                    }
                    else
                    {
                        if (previousMove.y > 0)
                        {
                            base.SetFacing('U');
                        }
                        else
                        {
                            base.SetFacing('D');
                        }
                    }

                    gameGrid.UndoMove(gridPosition, previousMove);
                    gameGrid.QueueLaserIOUpdate();
                }
            }
        }
    }
}