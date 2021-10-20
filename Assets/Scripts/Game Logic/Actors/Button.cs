using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Actor
{
    private List<GameObject> connectedTo;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Actor>() != null && collider.gameObject.GetComponent<Actor>().GetIsMovable())
        {
            ActivateConnectedActors();
        }
    }

    public void FindConnectedActors()
    {
        connectedTo = new List<GameObject>();
        List<GameObject> leftList = FindConnectedActorsHelper(gridPosition + Vector2Int.left, Vector2Int.left);
        List<GameObject> rightList = FindConnectedActorsHelper(gridPosition + Vector2Int.right, Vector2Int.right);
        List<GameObject> upList = FindConnectedActorsHelper(gridPosition + Vector2Int.down, Vector2Int.up);
        List<GameObject> downList = FindConnectedActorsHelper(gridPosition + Vector2Int.up, Vector2Int.down);
        foreach (GameObject o in leftList)
        {
            connectedTo.Add(o);
        }
        foreach (GameObject o in rightList)
        {
            connectedTo.Add(o);
        }
        foreach (GameObject o in upList)
        {
            connectedTo.Add(o);
        }
        foreach (GameObject o in downList)
        {
            connectedTo.Add(o);
        }
    }

    private List<GameObject> FindConnectedActorsHelper(Vector2Int targetPos, Vector2Int scanDir)
    {
        List<GameObject> occupants = gameGrid.GetGridSquareOccupants(targetPos);
        List<GameObject> connecteds = new List<GameObject>();

        foreach (GameObject o in occupants)
        {
            if (o.gameObject.transform.tag == "Wirable")
            {
                connecteds.Add(o);
            }
            else if (o.gameObject.transform.tag == "Wire")
            {
                Vector2Int newDir = o.GetComponent<Wire>().RotateWithWire(scanDir);
                if (!newDir.Equals(Vector2Int.zero))
                {
                    List<GameObject> futureConnecteds = FindConnectedActorsHelper(targetPos + new Vector2Int(newDir.x, -newDir.y), newDir);
                    foreach (GameObject c in futureConnecteds)
                    {
                        connecteds.Add(c);
                    }
                }

                connecteds.Add(o);
            }
        }

        return connecteds;
    }

    private void ActivateConnectedActors()
    {
        foreach(GameObject o in connectedTo)
        {
            if (o != null && o.GetComponent<Actor>() != null)
            {
                o.GetComponent<Actor>().PerformButtonAction();
            }
        }
    }
}
