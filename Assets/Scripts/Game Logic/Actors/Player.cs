using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Actor
{
    /** The keyboard key corresponding to the command to move the player character LEFT by one square. */
    public static KeyCode leftMovement = KeyCode.LeftArrow;
    /** The keyboard key corresponding to the command to move the player character RIGHT by one square. */
    public static KeyCode rightMovement = KeyCode.RightArrow;
    /** The keyboard key corresponding to the command to move the player character UP by one square. */
    public static KeyCode upMovement = KeyCode.UpArrow;
    /** The keyboard key corresponding to the command to move the player character DOWN by one square. */
    public static KeyCode downMovement = KeyCode.DownArrow;
    public static KeyCode undo = KeyCode.Z;
    public static KeyCode toMainMenu = KeyCode.Escape;

    private float inputTimerTotal = 0.15f;
    private float inputTimer = 0f;

    private bool isAlive = true;
    private bool canMove = true;

    /** The z coordinate tracks the total number of actors moved by the action. */
    private Stack<Vector3Int> moves = new Stack<Vector3Int>();

    /**
     * Called once every frame, runs the function that detects player input.
     */
    public override void Update()
    {
        playerInput();
        base.Update();
    }

    /**
     * Detects when the player presses a movement key, faces the player object in the direction pressed,
     * attempts to move the player in the direction pressed (although this will do nothing if blocked by a wall or similar),
     * as well as redrawing all lasers on the screen, since they may bounce differently after the move.
     */
    private void playerInput()
    {
        if(Input.GetKeyDown(toMainMenu))
        {
            SceneManager.LoadScene("Title Screen");
        }

        bool keyHeld = false;
        Sprite[] spritesInOrder = { leftSprite, rightSprite, upSprite, downSprite };
        char[] charsInOrder = { 'L', 'R', 'U', 'D' };
        KeyCode[] keycodesInOrder = { leftMovement, rightMovement, upMovement, downMovement };
        Vector2Int[] dirsInOrder = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

        if (isAlive && canMove)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(keycodesInOrder[i]))
                {
                    //Key has just been pressed...
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesInOrder[i];
                    base.SetFacing(charsInOrder[i]);
                    MovementInput(dirsInOrder[i]);
                }
                else if (Input.GetKey(keycodesInOrder[i]))
                {
                    //Key is being held...
                    if (inputTimer >= inputTimerTotal)
                    {
                        //Move anyway
                        gameObject.GetComponent<SpriteRenderer>().sprite = spritesInOrder[i];
                        base.SetFacing(charsInOrder[i]);
                        MovementInput(dirsInOrder[i]);
                        inputTimer = 0f;
                    }
                    else if (!keyHeld)
                    {
                        inputTimer += Time.deltaTime;
                    }

                    keyHeld = true;
                }
            }
        }

        if (Input.GetKeyDown(undo))
        {
            UndoInput();
        }
        else if (Input.GetKey(undo))
        {
            //Key is being held...
            if (inputTimer >= inputTimerTotal)
            {
                //Undo anyway
                UndoInput();
                inputTimer = 0f;
            }
            else if (!keyHeld)
            {
                inputTimer += Time.deltaTime;
            }

            keyHeld = true;
        }

        if (!keyHeld)
        {
            inputTimer = 0;
        }

        //Old badness
        /**if(gameGrid.GetLevelIsActive())
        {
            if (Input.GetKeyDown(leftMovement))
            {
                //LEFT MOVEMENT
                gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                base.SetFacing('L');
                MovementInput(Vector2Int.left);
            }
            else if (Input.GetKey(leftMovement))
            {
                //Left is being held...
                if (inputTimer >= inputTimerTotal)
                {
                    //Move anyway
                    gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                    base.SetFacing('L');
                    MovementInput(Vector2Int.left);
                    inputTimer = 0f;
                }
                else
                {
                    inputTimer += Time.deltaTime;
                }
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
        }
    }*/
    }

    private void MovementInput(Vector2Int dir)
    {
        int movedActorCount = gameGrid.MoveActorInGrid(gridPosition, dir, false);
        if (movedActorCount > 0)
        {
            moves.Push(new Vector3Int(dir.x, dir.y, movedActorCount));
        }
        gameGrid.QueueLaserIOUpdate();
    }

    private void UndoInput()
    {
        if (moves.Count > 0)
        {
            SetIsAlive(true);
            SetCanMove(true);

            Vector3Int previousMove = moves.Pop();

            if (previousMove.x > 0)
            {
                base.SetFacing('R');
            }
            else if (previousMove.x < 0)
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

    public void SetIsAlive(bool isAliveIn)
    {
        if(!isAliveIn)
        {
            GameObject ender = Instantiate(GameGrid.levelLoserPrefabStatic);
            ender.transform.position = new Vector3(GameGrid.sceneCameraStatic.transform.position.x, GameGrid.sceneCameraStatic.transform.position.y, 0);
        }
        isAlive = isAliveIn;
        GetComponent<SpriteRenderer>().enabled = isAliveIn;
        GetComponent<BoxCollider2D>().enabled = isAliveIn;
    }

    public void SetCanMove(bool canMoveIn)
    {
        canMove = canMoveIn;
    }
}