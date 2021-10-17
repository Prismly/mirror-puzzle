using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private KeyCode leftMovement = KeyCode.LeftArrow;
    private KeyCode rightMovement = KeyCode.RightArrow;
    private KeyCode upMovement = KeyCode.UpArrow;
    private KeyCode downMovement = KeyCode.DownArrow;

    public override void Update()
    {
        playerInput();
    }

    private void playerInput()
    {
        if (Input.GetKeyDown(leftMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
            gameGrid.MoveActorInGrid(gridPosition, GameGrid.Cardinal.LEFT, false);
        }
        if (Input.GetKeyDown(rightMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
            gameGrid.MoveActorInGrid(gridPosition, GameGrid.Cardinal.RIGHT, false);
        }
        if (Input.GetKeyDown(upMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
            gameGrid.MoveActorInGrid(gridPosition, GameGrid.Cardinal.UP, false);
        }
        if (Input.GetKeyDown(downMovement))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
            gameGrid.MoveActorInGrid(gridPosition, GameGrid.Cardinal.DOWN, false);
        }
    }
}