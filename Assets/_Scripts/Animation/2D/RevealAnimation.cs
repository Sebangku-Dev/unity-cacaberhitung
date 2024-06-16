using UnityEngine;
using System.Threading.Tasks;

public class RevealAnimation : BaseAnimation, IAnimate
{
    [SerializeField] Vector3 delta;

    private void Awake()
    {
        // Set initial state
        transform.LeanMoveLocal(transform.localPosition + delta, 0f);
    }

    private void OnEnable()
    {
        if (isAnimateOnLoad) Load();
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
