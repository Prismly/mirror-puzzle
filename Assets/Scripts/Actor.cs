﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected GameGrid gameGrid;
    protected Vector2Int facing;
    protected Vector2Int gridPosition;

    [SerializeField]
    protected Sprite leftSprite;
    [SerializeField]
    protected Sprite rightSprite;
    [SerializeField]
    protected Sprite upSprite;
    [SerializeField]
    protected Sprite downSprite;
    
    [SerializeField]
    private bool isMovable;
    [SerializeField]
    private bool isWall;
    [SerializeField]
    private bool isDeath;

    public void Start()
    {
        if (facing.x == 0)
        {
            if (facing.y > 0)
            {
                //UP
                gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
            }
            else
            {
                //DOWN
                gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
            }
        }
        else
        {
            if(facing.x > 0)
            {
                //RIGHT
                gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
            }
            else
            {
                //LEFT
                gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
            }
        }
    }

    public void SetGameGrid(GameGrid gameGrid)
    {
        this.gameGrid = gameGrid;
    }

    public Vector2Int GetFacing()
    {
        return facing;
    }

    public void SetFacing(char dirId)
    {
        switch(dirId)
        {
            case 'L':
                {
                    facing = Vector2Int.left;
                    break;
                }
            case 'R':
                {
                    facing = Vector2Int.right;
                    break;
                }
            case 'U':
                {
                    facing = Vector2Int.up;
                    break;
                }
            case 'D':
                {
                    facing = Vector2Int.down;
                    break;
                }
        }
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public bool GetIsMovable()
    {
        return isMovable;
    }

    public bool GetIsWall()
    {
        return isWall;
    }

    public bool GetIsDeath()
    {
        return isDeath;
    }

    public void UpdateActorPos(Vector2Int newPos)
    {
        gridPosition = newPos;
        gameObject.transform.position = new Vector3(gridPosition.x, -gridPosition.y);
        Physics.SyncTransforms();
    }
}
