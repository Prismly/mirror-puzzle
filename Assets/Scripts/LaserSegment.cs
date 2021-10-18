using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : MonoBehaviour
{
    private LaserOut outputTile;

    public void SetOutputTile(LaserOut outputTileIn)
    {
        outputTile = outputTileIn;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.name);
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("You died lol");
            Destroy(collider.gameObject);
        }
    }
}
