using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAnimationSync : MonoBehaviour
{
    private static float animationFrameIdx = 0;
    private static float increment = 0.01f;

    // Update is called once per frame
    void Update()
    {
        animationFrameIdx += increment;
        if(animationFrameIdx > 3)
        {
            animationFrameIdx = 0;
        }
    }

    public static int GetAnimFrame(bool isVertical)
    {
        if(!isVertical)
        {
            return Mathf.FloorToInt(animationFrameIdx);
        }
        else
        {
            return Mathf.FloorToInt(animationFrameIdx + 4);
        }
    }
}
