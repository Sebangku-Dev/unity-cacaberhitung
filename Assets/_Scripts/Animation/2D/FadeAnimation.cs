
using UnityEngine;

public class FadeAnimation : BaseAnimation, IAnimate
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] float targetAlpha;

    private void Start()
    {
        if (isAnimateOnLoad) Load();
    }

    public void Load()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        canvasGroup.LeanAlpha(targetAlpha > 0.0f ? targetAlpha : 0.6f, duration).setDelay(delay);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        transform.localScale = Vector3.one;
        int id = canvasGroup.LeanAlpha(0f, duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }


}
