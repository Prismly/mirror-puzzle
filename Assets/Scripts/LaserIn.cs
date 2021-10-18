using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserIn : Actor
{
    private bool isLit = false;

    [SerializeField]
    protected Sprite leftLitSprite;
    [SerializeField]
    protected Sprite rightLitSprite;
    [SerializeField]
    protected Sprite upLitSprite;
    [SerializeField]
    protected Sprite downLitSprite;

    public bool GetIsLit()
    {
        return isLit;
    }

    public void SetIsLit(bool isLitIn)
    {
        isLit = isLitIn;
        UpdateSprite();
    }

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
