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
        if(Input.GetKeyDown(leftMovement))
        {
            gameGrid.MoveActor(gridPosition, GameGrid.Cardinal.LEFT, false);
        }
        if(Input.GetKeyDown(rightMovement))
        {
            gameGrid.MoveActor(gridPosition, GameGrid.Cardinal.RIGHT, false);
        }
        if(Input.GetKeyDown(upMovement))
        {
            gameGrid.MoveActor(gridPosition, GameGrid.Cardinal.UP, false);
        }
        if(Input.GetKeyDown(downMovement))
        {
            gameGrid.MoveActor(gridPosition, GameGrid.Cardinal.DOWN, false);
        }
    }

    private void playerInput()
    {
        if (Input.GetKeyDown(leftMovement))
        {

        }
    }
}