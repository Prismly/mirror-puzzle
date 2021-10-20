using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : Actor
{
    /** The laser output actor that spawned this laser segment. */
    private LaserOut outputTile;
    [SerializeField] private Sprite[] animationFrames;
    private bool isVertical;

    public void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = animationFrames[LaserAnimationSync.GetAnimFrame(isVertical)];
    }

    public override void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = animationFrames[LaserAnimationSync.GetAnimFrame(isVertical)];
        base.Update();
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

    ///**
    // * Runs whenever something gets in the way of this laser segment. 
    // * @param collider - the collider we've collided with.
    // */
    //public void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if(collider.gameObject.tag == "Player")
    //    {
    //        collider.gameObject.GetComponent<Player>().SetIsAlive(false);
    //        outputTile.UpdateLasers();
    //    }
    //}
}
