using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private KeyCode leftMovement = KeyCode.LeftArrow;
    private KeyCode rightMovement = KeyCode.RightArrow;
    private KeyCode upMovement = KeyCode.UpArrow;
    private KeyCode downMovement = KeyCode.DownArrow;

    public void Update()
    {
        playerInput();
    }

    private void playerInput()
    {
        if (Input.GetKeyDown(leftMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
            base.SetFacing('L');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.left, false);
        }
        if (Input.GetKeyDown(rightMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
            base.SetFacing('R');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.right, false);
        }
        if (Input.GetKeyDown(upMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
            base.SetFacing('U');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.up, false);
        }
        if (Input.GetKeyDown(downMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
            base.SetFacing('D');
            gameGrid.MoveActorInGrid(gridPosition, Vector2Int.down, false);
        }
    }
}