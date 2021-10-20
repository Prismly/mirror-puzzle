using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Actor
{
    public override void PerformButtonAction()
    {
        if (facing.Equals(Vector2Int.left))
        {
            SetFacing('R');
            isStop = false;
        }
        else if (facing.Equals(Vector2Int.right))
        {
            SetFacing('L');
            isStop = true;
        }
        else if (facing.Equals(Vector2Int.up))
        {
            SetFacing('D');
            isStop = false;
        }
        else
        {
            SetFacing('U');
            isStop = true;
        }

        base.PerformButtonAction();
    }

    public override void SetFacing(char dirId)
    {
        base.SetFacing(dirId);

        isStop = facing.Equals(Vector2Int.left) || facing.Equals(Vector2Int.up);
    }
}
