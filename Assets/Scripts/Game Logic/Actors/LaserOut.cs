using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOut : Actor
{
    /** The prefab that can be used to instantiate a new laser segment object. */
    [SerializeField] private GameObject laserSegmentPrefab;
    /** A list of all laserSegments currently in existence that are associated with this laser output actor. */
    private List<GameObject> laserSegments = new List<GameObject>();

    private float laserSegmentStartpointOffset = 0.25f;

    private bool isOn = true;

    private static int orderInLayer = 0;

    private void FireLaser(Vector2Int originPos, Vector2Int dir, bool ignoreStartingSquare)
    {
        Vector2Int currentPos = originPos;

        if(ignoreStartingSquare)
        {
            currentPos = GenerateSegment(currentPos, dir);
        }

        while(!ContainsSolid(currentPos) && !ContainsMirror(currentPos))
        {
            currentPos = GenerateSegment(currentPos, dir);
        }

        //By this point, currentPos should point to the tile with the first interactable actor.
        //If we hit something solid, we're done. If we hit a mirror and only a mirror, though...

        if(!ContainsSolid(currentPos) && ContainsMirror(currentPos))
        {
            Vector2Int newDir = RotateWithMirror(GetFirstMirror(currentPos), dir);
            if (!newDir.Equals(Vector2Int.zero))
            {
                GenerateSegment(currentPos, dir);
                FireLaser(currentPos, newDir, true);
            }
        }

        List<GameObject> occupants = gameGrid.GetGridSquareOccupants(currentPos);
    }

    private Vector2Int GenerateSegment(Vector2Int currentPos, Vector2Int dir)
    {
        GameObject newSegment = Instantiate(laserSegmentPrefab);
        newSegment.transform.position = new Vector3(currentPos.x, -currentPos.y, 0);
        newSegment.GetComponent<LaserSegment>().SetIsVertical(dir.x == 0);
        newSegment.GetComponent<LaserSegment>().SetGridPosition(currentPos);
        gameGrid.AddActorToGridSquare(newSegment, currentPos);
        laserSegments.Add(newSegment);
        return new Vector2Int(currentPos.x + dir.x, currentPos.y - dir.y);
    }

    private bool ContainsSolid(Vector2Int targetPos)
    {
        List<GameObject> occupants = gameGrid.GetGridSquareOccupants(targetPos);

        foreach(GameObject o in occupants)
        {
            if(o != null && o.GetComponent<Actor>() != null && o.GetComponent<Actor>().GetIsStop())
            {
                return true;
            }
        }
        return false;
    }

    private bool ContainsMirror(Vector2Int targetPos)
    {
        List<GameObject> occupants = gameGrid.GetGridSquareOccupants(targetPos);

        foreach (GameObject o in occupants)
        {
            if (o != null && o.tag == "Mirror")
            {
                return true;
            }
        }
        return false;
    }

    private GameObject GetFirstMirror(Vector2Int targetPos)
    {
        List<GameObject> occupants = gameGrid.GetGridSquareOccupants(targetPos);

        foreach (GameObject o in occupants)
        {
            if (o != null && o.tag == "Mirror")
            {
                return o;
            }
        }
        return null;
    }

    /**
     * Given the direction from which a laser has collided with a mirror, returns the direction it will bounce.
     * Mirrors can redirect lasers if they collide on one of two adjacent edges, specified by the mirror's "facing" field.
     * The two edges that will result in a redirection are the one in the "facing" field, and the one 90 degrees clockwise from it.
     * @param mirror - the mirror actor we want to rotate with.
     * @param laserDir - the direction in which the current laser segment is traveling.
     * @returns the direction in which the laser will be redirected; if it will not be redirected because it is hitting the wrong side, returns Vector2.zero.
     */
    private Vector2Int RotateWithMirror(GameObject mirror, Vector2 laserDir)
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
            return Vector2Int.zero;
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
        CleanLaserList();

        foreach (GameObject o in laserSegments)
        {
            gameGrid.RemoveActorFromGridSquare(o, o.GetComponent<LaserSegment>().GetGridPosition());
            Destroy(o);
        }

        if (isOn)
        {
            FireLaser(new Vector2Int(gridPosition.x, gridPosition.y), facing, false);
        }
    }

    public void CleanLaserList()
    {
        for(int i = 0; i < laserSegments.Count; i++)
        {
            if(laserSegments[i] == null)
            {
                laserSegments.RemoveAt(i);
                i--;
            }
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
