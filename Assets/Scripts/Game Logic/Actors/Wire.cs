using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : Actor
{
    [SerializeField] private Sprite[] spritesheet = new Sprite[8];
    private Vector2Int dir1;
    private Vector2Int dir2;
    private bool isLit;
    private char facingId;

    public override void SetFacing(char dirId)
    {
        facingId = dirId;

        switch (dirId)
        {
            case 'L':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[1];
                    isLit = true;
                    dir1 = Vector2Int.left;
                    dir2 = Vector2Int.up;
                    break;
                }
            case 'R':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[3];
                    isLit = true;
                    dir1 = Vector2Int.right;
                    dir2 = Vector2Int.down;
                    break;
                }
            case 'U':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[5];
                    isLit = true;
                    dir1 = Vector2Int.up;
                    dir2 = Vector2Int.right;
                    break;
                }
            case 'D':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[7];
                    isLit = true;
                    dir1 = Vector2Int.down;
                    dir2 = Vector2Int.left;
                    break;
                }
            case 'H':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
                    isLit = true;
                    dir1 = Vector2Int.left;
                    dir2 = Vector2Int.right;
                    break;
                }
            case 'V':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
                    isLit = true;
                    dir1 = Vector2Int.up;
                    dir2 = Vector2Int.down;
                    break;
                }
            case 'l':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[0];
                    isLit = false;
                    dir1 = Vector2Int.left;
                    dir2 = Vector2Int.up;
                    break;
                }
            case 'r':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[2];
                    isLit = false;
                    dir1 = Vector2Int.right;
                    dir2 = Vector2Int.down;
                    break;
                }
            case 'u':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[4];
                    isLit = false;
                    dir1 = Vector2Int.up;
                    dir2 = Vector2Int.right;
                    break;
                }
            case 'd':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[6];
                    isLit = false;
                    dir1 = Vector2Int.down;
                    dir2 = Vector2Int.left;
                    break;
                }
            case 'h':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                    isLit = false;
                    dir1 = Vector2Int.left;
                    dir2 = Vector2Int.right;
                    break;
                }
            case 'v':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
                    isLit = false;
                    dir1 = Vector2Int.up;
                    dir2 = Vector2Int.down;
                    break;
                }
        }
    }

    public override void PerformButtonAction()
    {
        if(char.IsLower(facingId))
        {
            SetFacing(char.ToUpper(facingId));
        }
        else
        {
            SetFacing(char.ToLower(facingId));
        }
    }

    public Vector2Int RotateWithWire(Vector2Int directionOfScan)
    {
        Vector2Int antiDirectionOfScan = new Vector2Int(-directionOfScan.x, -directionOfScan.y);

        if(antiDirectionOfScan.Equals(dir1))
        {
            return dir2;
        }
        else if(antiDirectionOfScan.Equals(dir2))
        {
            return dir1;
        }
        else
        {
            return Vector2Int.zero;
        }
    }
}
