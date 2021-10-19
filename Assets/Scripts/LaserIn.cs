using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserIn : Actor
{
    /** Flag specifying whether this laser input currently has a laser pointed at it. */
    private bool isLit = false;

    /** The sprite for a left-facing, lit laser input. */
    [SerializeField] protected Sprite leftLitSprite;
    /** The sprite for a right-facing, lit laser input. */
    [SerializeField] protected Sprite rightLitSprite;
    /** The sprite for a upward-facing, lit laser input. */
    [SerializeField] protected Sprite upLitSprite;
    /** The sprite for a downward-facing, lit laser input. */
    [SerializeField] protected Sprite downLitSprite;

    /**
     * Gets the flag that specifies whether this laser input currently has a laser pointed at it.
     * @return whether or not this laser input is contributing to winning the level.
     */
    public bool GetIsLit()
    {
        return isLit;
    }

    /**
     * Sets the flag that specifies whether this laser input currently has a laser pointed at it.
     * @param isLitIn - the new value for the flag.
     */
    public void SetIsLit(bool isLitIn)
    {
        isLit = isLitIn;
        UpdateSprite();
    }

    /**
     * Updates the sprite currently being used by this laser input based on whether it is lit, and also
     * the direction it is facing.
     */
    private void UpdateSprite()
    {
        if (facing.Equals(Vector2Int.left))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = isLit ? leftLitSprite : leftSprite;
        }
        else if(facing.Equals(Vector2Int.right))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = isLit ? rightLitSprite : rightSprite;
        }
        else if (facing.Equals(Vector2Int.up))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = isLit ? upLitSprite : upSprite;
        }
        else if (facing.Equals(Vector2Int.down))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = isLit ? downLitSprite : downSprite;
        }
    }
}
