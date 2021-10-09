using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private Cardinal facing;
    private Vector2Int gridPosition;
    private bool isMovable;
    private GameObject actorObject;

    public enum Cardinal
    { 
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public Actor(Vector2Int gridPosition, Cardinal facing, bool isMovable)
    {
        this.gridPosition = gridPosition;
        this.facing = facing;
        this.isMovable = isMovable;
    }
}
