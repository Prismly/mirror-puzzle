using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : MonoBehaviour
{
    /** The laser output actor that spawned this laser segment. */
    private LaserOut outputTile;

    /**
     * Sets the laser output actor that spawned this laser segment.
     * @param outputTileIn - the actor to assign to outputTile.
     */
    public void SetOutputTile(LaserOut outputTileIn)
    {
        outputTile = outputTileIn;
    }

    /**
     * Runs whenever something gets in the way of this laser segment. 
     * @param collider - the collider we've collided with.
     */
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<Player>().SetIsAlive(false);
            outputTile.UpdateLasers();
        }
    }
}
