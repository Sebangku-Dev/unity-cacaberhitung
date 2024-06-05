using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanAnimation : BaseAnimation, IAnimate
{
    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Pan(int delta)
    {
        transform.LeanMoveLocalY(transform.localPosition.y + delta, duration).setEaseInOutQuart().setDelay(delay);
    }

    public void Load()
    {
        throw new System.NotImplementedException();
    }
}
