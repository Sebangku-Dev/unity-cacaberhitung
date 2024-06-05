using UnityEngine;

public class Door : BaseAnimation, IAnimate
{
    public void Load()
    {
        throw new System.NotImplementedException();
    }

    public void MoveSlightly(int delta)
    {
        transform.LeanMoveLocalX(transform.localPosition.x + delta, duration).setEaseInOutQuart().setDelay(delay);
    }

    public void Close()
    {
        throw new System.NotImplementedException();
    }

}
