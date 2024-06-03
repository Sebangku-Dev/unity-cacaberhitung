using Unity.VisualScripting;
using UnityEngine;

public class Plate : BaseAnimation
{
    [Header("Main")]
    [SerializeField] Direction animationDirection;
    [SerializeField] Vector3 initialLocalPosition;

    private int distanceFromInitialPosition = 1200;

    public void Load()
    {
        initialLocalPosition = transform.localPosition;

        switch (animationDirection)
        {
            case Direction.Right:
                transform.localPosition += Vector3.right * distanceFromInitialPosition;
                LeanTween.moveLocalX(gameObject, initialLocalPosition.x, duration).setEaseInOutQuart().setDelay(delay);
                break;
            case Direction.Left:
                transform.localPosition += Vector3.left * distanceFromInitialPosition;
                LeanTween.moveLocalX(gameObject, initialLocalPosition.x, duration).setEaseInOutQuart().setDelay(delay);
                break;
            default:
                break;
        }
    }

    public void Close()
    {
        int id = 0;

        switch (animationDirection)
        {
            case Direction.Right:
                id = LeanTween.moveLocalX(gameObject, distanceFromInitialPosition, duration).id;
                break;
            case Direction.Left:
                id = LeanTween.moveLocalX(gameObject, -distanceFromInitialPosition, duration).id;
                break;
            default:
                break;
        }

        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setEase(LeanTweenType.easeInOutQuart).setOnComplete(() =>
            {
                transform.parent.gameObject.SetActive(false);
            });
        }
    }
}
