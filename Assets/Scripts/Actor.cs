using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected GameGrid gameGrid;
    protected GameGrid.Cardinal facing;
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
    private bool isSolid;

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

    public bool GetIsSolid()
    {
        return isSolid;
    }

    public void UpdateActorInfo(Vector2Int newPos, GameObject newContainer)
    {
        gridPosition = newPos;
        gameObject.transform.parent = newContainer.transform;
        gameObject.transform.position = new Vector3(gridPosition.x, -gridPosition.y);
    }
}
