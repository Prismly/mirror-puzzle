using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOut : Actor
{
    /** The prefab that can be used to instantiate a new laser segment object. */
    [SerializeField] private GameObject laserSegmentPrefab;
    /** A list of all laserSegments currently in existence that are associated with this laser output actor. */
    private List<GameObject> laserSegments = new List<GameObject>();

    /** The sprite for a vertical laser segment. */
    [SerializeField] private Sprite laserVertical;
    /** The sprite for a horizontal laser segment. */
    [SerializeField] private Sprite laserHorizontal;

    private bool isOn = true;

    /**
     * "Fires" a laser from the specified origin point in the given direction, creating an object called a laser segment.
     * Visually, the created laser segment should extend from the origin square to the first stopping actor OR the first mirror actor it encounters.
     * If the laser extends to a mirror actor in the proper direction, it will be redirected, and this method will be recursively called with the
     * mirror as the new origin.
     * @param origin - the origin point of the raycast to use to generate a laser segment.
     * @param dir - the direction in which to fire the laser.
     * @param ignoreStartingSquare - whether or not to take into account colliders on the same GridSquare as origin.
     */
    private void FireLaser(Vector2Int origin, Vector2 dir, bool ignoreStartingSquare)
    {
        //Get all colliders in the direction of the laser.
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir);
        int hitsIndex = 0;
        if(ignoreStartingSquare)
        {
            //If the ignoreStartingSquare flag is set, specifically check all hits until one that both fits the requirements AND isn't on the same grid position as origin is found.

            //The current hit is a valid endpoint if...
            //a) the thing we are colliding with is stopping, like a wall or a closed door. Anything the player cannot move through.
            //b) OR the thing we are colliding with is a mirror, which has a possibility of redirecting the laser.
            //c) (ignoreStartingSquare exclusive) regardless of the other two, its grid position must not match that of this laser's origin.
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.GetComponent<Actor>())
                {
                    Vector2Int currentHitPosition = new Vector2Int(hits[i].collider.gameObject.GetComponent<Actor>().GetGridPosition().x,
                    -hits[i].collider.gameObject.GetComponent<Actor>().GetGridPosition().y);

                    if (!currentHitPosition.Equals(origin) && (hits[i].collider.gameObject.GetComponent<Actor>().GetIsStop() ||
                        hits[i].collider.gameObject.tag == "Mirror"))
                    {
                        hitsIndex = i;
                        break;
                    }
                }
            }
        }
        else
        {
            //If the ignoreStartingSquare flag is not set, check all hits until one that fits both requirements is found, regardless of grid position.

            //The current hit is a valid collision if...
            //a) the thing we are colliding with is stopping, like a wall or a closed door. Anything the player cannot move through.
            //b) OR the thing we are colliding with is a mirror, which has a possibility of redirecting the laser.
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.GetComponent<Actor>())
                {
                    if (hits[i].collider.gameObject.GetComponent<Actor>().GetIsStop() || hits[i].collider.gameObject.tag == "Mirror")
                    {
                        hitsIndex = i;
                        break;
                    }
                }
            }
        }

        //If we found a suitable hit...
        if(hitsIndex < hits.Length)
        {
            RaycastHit2D hit = hits[hitsIndex];

            //Generate a laser object for this line.
            GameObject newSegment = Instantiate(laserSegmentPrefab);
            newSegment.GetComponent<LaserSegment>().SetOutputTile(this);

            Vector2 dirAbs = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            //Results in a vector that has a zero dimension and a non-zero dimension, representing the length of the laser.
            Vector2 size = dirAbs * (hit.distance + 0.5f - gameGrid.GetColliderReductionOffset());
            //Corrects for whichever dimension is of length 0, resulting in the correct size of the sprite and collider.
            size = size + Vector2.one - dirAbs;

            newSegment.GetComponent<SpriteRenderer>().size = size;
            if (dir.x == 0)
            {
                //Laser is being fired vertically
                newSegment.GetComponent<SpriteRenderer>().sprite = laserVertical;
            }
            else
            {
                //Laser is being fired horizontally
                newSegment.GetComponent<SpriteRenderer>().sprite = laserHorizontal;
            }

            newSegment.GetComponent<BoxCollider2D>().size = new Vector2(size.x - gameGrid.GetColliderReductionOffset(), size.y - gameGrid.GetColliderReductionOffset());

            Vector3 oldPos = new Vector3(origin.x, origin.y, 0);
            Vector3 offset = new Vector3(dir.x * ((hit.distance - 0.5f) / 2), dir.y * ((hit.distance - 0.5f) / 2), 0);

            newSegment.transform.position = oldPos + offset;

            laserSegments.Add(newSegment);

            Vector2 redirectionDir = RotateWithMirror(hit.collider.gameObject, dir);

            if (hit.collider.gameObject.tag == "Mirror" && !redirectionDir.Equals(Vector2.zero))
            {
                //If the laser should be redirected from its collision with a mirror...

                Vector2Int newOrigin = new Vector2Int(hit.collider.gameObject.GetComponent<Actor>().GetGridPosition().x,
                    -hit.collider.gameObject.GetComponent<Actor>().GetGridPosition().y);

                //...fire the laser again, this time starting from the mirror and in the new direction.
                FireLaser(newOrigin, redirectionDir, true);
            }
            else
            {
                //If something stopped the laser in its tracks, check for a laser input object to turn on.

                List<GameObject> collidedOccupants = gameGrid.GetGridSquareOccupants(hit.collider.gameObject.GetComponent<Actor>().GetGridPosition());
                for(int i = 0; i < collidedOccupants.Count; i++)
                {
                    LaserIn laserInComponent = collidedOccupants[i].GetComponent<LaserIn>();

                    if (laserInComponent != null)
                    {
                        //If this occupant of the tile we've collided with is a laser input...
                        Vector2Int inputDir = new Vector2Int(-laserInComponent.GetFacing().x, -laserInComponent.GetFacing().y);
                        if(dir.Equals(inputDir))
                        {
                            laserInComponent.SetIsLit(true);
                        }
                    }
                }
            }
        }
    }

    /**
     * Given the direction from which a laser has collided with a mirror, returns the direction it will bounce.
     * Mirrors can redirect lasers if they collide on one of two adjacent edges, specified by the mirror's "facing" field.
     * The two edges that will result in a redirection are the one in the "facing" field, and the one 90 degrees clockwise from it.
     * @param mirror - the mirror actor we want to rotate with.
     * @param laserDir - the direction in which the current laser segment is traveling.
     * @returns the direction in which the laser will be redirected; if it will not be redirected because it is hitting the wrong side, returns Vector2.zero.
     */
    private Vector2 RotateWithMirror(GameObject mirror, Vector2 laserDir)
    {
        Vector2Int mirrorDir1 = mirror.GetComponent<Actor>().GetFacing();
        Vector2Int mirrorDir2;

        //Sets mirrorDir2 to the direction 90 degrees CLOCKWISE from mirrorDir1.
        if(mirrorDir1.x != 0)
        {
            mirrorDir2 = new Vector2Int(mirrorDir1.x - mirrorDir1.x, mirrorDir1.y - mirrorDir1.x);
        }
        else
        {
            mirrorDir2 = new Vector2Int(mirrorDir1.x + mirrorDir1.y, mirrorDir1.y - mirrorDir1.y);
        }

        if(laserDir.Equals(new Vector2Int(-mirrorDir1.x, -mirrorDir1.y)))
        {
            return mirrorDir2;
        }
        else if(laserDir.Equals(new Vector2Int(-mirrorDir2.x, -mirrorDir2.y)))
        {
            return mirrorDir1;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public override void SetFacing(char dirId)
    {
        isOn = char.IsUpper(dirId);
        switch (char.ToUpper(dirId))
        {
            case 'L':
                {
                    facing = Vector2Int.left;
                    gameObject.GetComponent<SpriteRenderer>().sprite = leftSprite;
                    break;
                }
            case 'R':
                {
                    facing = Vector2Int.right;
                    gameObject.GetComponent<SpriteRenderer>().sprite = rightSprite;
                    break;
                }
            case 'U':
                {
                    facing = Vector2Int.up;
                    gameObject.GetComponent<SpriteRenderer>().sprite = upSprite;
                    break;
                }
            case 'D':
                {
                    facing = Vector2Int.down;
                    gameObject.GetComponent<SpriteRenderer>().sprite = downSprite;
                    break;
                }
        }
    }

    /**
     * Destroys all laserSegments associated with this laser output actor and fires a new laser to redraw the laser's path.
     */
    public void UpdateLasers()
    {
        foreach (GameObject o in laserSegments)
        {
            Destroy(o);
        }

        if (isOn)
        {
            FireLaser(new Vector2Int(gridPosition.x, -gridPosition.y), facing, false);
        }
    }

    public override void SelfDestruct()
    {
        foreach (GameObject o in laserSegments)
        {
            Destroy(o);
        }
        base.SelfDestruct();
    }

    public override void PerformButtonAction()
    {
        isOn = !isOn;
        UpdateLasers();
    }
}
