using Unity.VisualScripting;
using UnityEngine;

public class Plate : BaseAnimation, IAnimate
{
    [Header("Main")]
    [SerializeField] Vector3 delta;

    private void Start()
    {
        if (isAnimateOnLoad) Load();

        transform.LeanMoveLocal(transform.localPosition + delta, 0f);
    }

    public void Load()
    {
        gameObject.SetActive(true);

        transform.LeanMoveLocal(transform.localPosition - delta, duration).setEaseOutExpo().setDelay(delay);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        int id = transform.LeanMoveLocal(transform.localPosition + delta, duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }
}
