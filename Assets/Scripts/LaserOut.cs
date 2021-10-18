using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOut : Actor
{
    [SerializeField]
    private GameObject laserSegmentPrefab;
    private List<GameObject> laserSegments = new List<GameObject>();

    [SerializeField]
    private Sprite laserVertical;
    [SerializeField]
    private Sprite laserHorizontal;

    public void FixedUpdate()
    {
        UpdateLasers();
    }

    private void FireLaser(Vector2Int origin, Vector2 dir, bool ignoreStartingSquare)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir);
        int hitsIndex = 0;
        if(ignoreStartingSquare)
        {
            Vector2Int currentHitPosition = new Vector2Int(hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetGridPosition().x,
                -hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetGridPosition().y);

            while (hitsIndex < hits.Length && currentHitPosition.Equals(origin) || 
                (!hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetIsWall() && hits[hitsIndex].collider.gameObject.tag != "Mirror"))
            {
                hitsIndex++;
                currentHitPosition = new Vector2Int(hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetGridPosition().x,
                    -hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetGridPosition().y);
            }
        }
        else
        {
            while(hitsIndex < hits.Length && !hits[hitsIndex].collider.gameObject.GetComponent<Actor>().GetIsWall() && hits[hitsIndex].collider.gameObject.tag != "Mirror")
            {
                hitsIndex++;
            }
        }

        if(hitsIndex < hits.Length)
        {
            RaycastHit2D hit = hits[hitsIndex];
            Vector2 dirAbs = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            //If a suitable collider was found, generate a laser object for this line.
            GameObject newSegment = Instantiate(laserSegmentPrefab);
            newSegment.GetComponent<LaserSegment>().SetOutputTile(this);

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

            //Debug.Log("Offset: " + offset);
            //Debug.Log("Dir: " + dir);
            //Debug.Log("thing: " + (hit.distance - 0.5f) / 2);

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
     *  Given the direction from which a laser has collided with a mirror, returns the direction it will bounce.
     *  Mirrors can redirect lasers if they collide on one of two adjacent edges, specified by the mirror's "facing" field.
     *  The two edges that will result in a redirection are the one in the "facing" field, and the one 90 degrees clockwise from it.
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
            return new Vector2(0, 0);
        }
    }

    public void UpdateLasers()
    {
        foreach (GameObject o in laserSegments)
        {
            Destroy(o);
        }

        FireLaser(new Vector2Int(gridPosition.x, -gridPosition.y), facing, false);
    }
}
