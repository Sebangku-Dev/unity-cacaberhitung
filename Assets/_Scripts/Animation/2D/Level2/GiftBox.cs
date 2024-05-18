using System.Threading.Tasks;
using UnityEngine;

public class GiftBox : BaseAnimation
{
    [Header("Main")]
    [SerializeField] Transform lid;
    [SerializeField] Direction animationDirection;
    [SerializeField] private bool isLidClosed;
    [SerializeField] private Vector3 originPosition;


    public void Load()
    {
        transform.localScale = Vector3.zero;
        lid.localScale = Vector3.zero;

        LeanTween.scale(lid.gameObject, Vector3.one, duration).setEaseInOutQuart().setDelay(delay + 0.1f);
        LeanTween.scale(gameObject, Vector3.one, duration).setEaseInOutQuart().setDelay(delay);

        // Open lid
        if (!isLidClosed)
        {
            switch (animationDirection)
            {
                case Direction.Left:
                    LeanTween.moveLocalX(lid.gameObject, lid.position.x + -1000, duration).setEaseInOutQuart().setDelay(delay + duration);
                    break;
                case Direction.Right:
                    LeanTween.moveLocalX(lid.gameObject, lid.position.x + 1000, duration).setEaseInOutQuart().setDelay(delay + duration);
                    break;
                default:
                    break;

            }
        }
    }

    public void Close()
    {
        int id = LeanTween.moveLocalY(gameObject, -1800, duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setEase(LeanTweenType.easeInOutQuart).setOnComplete(() =>
            {
                // Reset the question sprite after complete
                transform.parent.gameObject.SetActive(false);
                LeanTween.moveLocalY(gameObject, originPosition.y, 0f);
                switch (animationDirection)
                {
                    case Direction.Left:
                        LeanTween.moveLocalX(lid.gameObject, 0, 0f).setEaseInOutQuart();
                        break;
                    case Direction.Right:
                        LeanTween.moveLocalX(lid.gameObject, 0, 0f).setEaseInOutQuart();
                        break;
                    default:
                        break;

                }
            });
        }
    }

    [System.Serializable]
    public enum Direction
    {
        Left, Right
    }
}
