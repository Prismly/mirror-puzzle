using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : MonoBehaviour
{
    /** The laser output actor that spawned this laser segment. */
    private LaserOut outputTile;
    [SerializeField] private Sprite[] animationFrames;
    private bool isVertical;

    public void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = animationFrames[LaserAnimationSync.GetAnimFrame(isVertical)];
    }

    public void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = animationFrames[LaserAnimationSync.GetAnimFrame(isVertical)];
    }

    /**
     * Sets the laser output actor that spawned this laser segment.
     * @param outputTileIn - the actor to assign to outputTile.
     */
    public void SetOutputTile(LaserOut outputTileIn)
    {
        outputTile = outputTileIn;
    }

    public void SetIsVertical(bool newVal)
    {
        isVertical = newVal;
    }

    /**
     * Runs whenever something gets in the way of this laser segment. 
     * @param collider - the collider we've collided with.
     */
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            //Debug.Log(collider.transform.position + (Vector3)collider.gameObject.GetComponent<BoxCollider2D>().offset);
            //Debug.Log(collider.gameObject.GetComponent<BoxCollider2D>().size);
            //Debug.Log(transform.position + (Vector3)gameObject.GetComponent<BoxCollider2D>().offset);
            //Debug.Log(gameObject.GetComponent<BoxCollider2D>().size);
            Debug.Log(StackTraceUtility.ExtractStackTrace());
            collider.gameObject.GetComponent<Player>().SetIsAlive(false);
            outputTile.UpdateLasers();
        }
    }
}
