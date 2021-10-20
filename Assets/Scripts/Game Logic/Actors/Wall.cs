using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Actor
{
    /** The spritesheet containing every variation of a wall. */
    [SerializeField] private Sprite[] spritesheet = new Sprite[30];

    /**
     * Overrides the SetFacing function in Actor, but accomplishes the same purpose.
     * Sets the value of the facing field for this Wall Actor, and sets the appropriate sprite from the
     * spritesheet based on the character passed in from the level layout text file.
     * @param wallSpriteId - the character read in from the level's text file, which specifies not the facing
     * direction but instead which sprite from the spritesheet to use.
     */
    public override void SetFacing(char wallSpriteId)
    {
        //The facing Vector is vestigial for walls, as they only face multiple directions cosmetically and will always be solid from every direction.
        facing = Vector2Int.left;

        //Walls come in many more varieties than a standard four-directional game object.
        //'L', 'R', 'U', and 'D' refer to the first four sprites in the spritesheet, the ones with three walls and an empty face.
        //The direction of these four exceptions is consistent with the middle wall edge - so the 'L' sprite has an empty face on the right edge.
        //The rest of the sprites will sequentially be referred to by lowercase letters and are stored in the spritesheet array: 'a' for the fifth, 'b' for the sixth, and so on up to 30.

        //Explicitly check for the four exceptions first...
        switch (wallSpriteId)
        {
            case 'L':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                    break;
                }
            case 'R':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
                    break;
                }
            case 'U':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
                    break;
                }
            case 'D':
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
                    break;
                }
            default:
                {
                    //...then generally define which characters go with which remaining sprites.
                    gameObject.GetComponent<SpriteRenderer>().sprite = spritesheet[wallSpriteId - 'a'];
                    break;
                }
        }
    }
}
