using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected GameGrid gameGrid;
    protected GameGrid.Cardinal facing;
    protected Vector2Int gridPosition;
    
    [SerializeField]
    private bool isMovable;

    public virtual void Update()
    {
        
    }

    public void SetGameGrid(GameGrid gameGrid)
    {
        this.gameGrid = gameGrid;
    }

    public void SetFacing(GameGrid.Cardinal facing)
    {
        this.facing = facing;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public bool GetIsMoveable()
    {
        return isMovable;
    }
}
