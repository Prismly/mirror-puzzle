using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor
{
    private Cardinal facing;
    private Vector2Int gridPosition;
    public enum Cardinal
    { 
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public Actor(Vector2Int gridPosition, Cardinal facing)
    {
        this.gridPosition = gridPosition;
        this.facing = facing;
    }
}
