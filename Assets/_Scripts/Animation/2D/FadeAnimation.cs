
using UnityEngine;

public class FadeAnimation : BaseAnimation, IAnimate
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] float targetAlpha;
    [SerializeField] bool isPulsing;

    LTDescr animatedCanvasGroup;

    private void OnEnable()
    {
        if (isAnimateOnLoad) Load();
    }

    public void Load()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        var _animatedCanvasGroup = canvasGroup.LeanAlpha(targetAlpha > 0.0f ? targetAlpha : 0.6f, duration).setDelay(delay);

        if (isPulsing)
        {
            _animatedCanvasGroup.setOnComplete(() => {
                animatedCanvasGroup = canvasGroup.LeanAlpha(0f, duration).setLoopPingPong();
            });
        }

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        if(isPulsing && animatedCanvasGroup != null) LeanTween.cancel(animatedCanvasGroup.id);

        int id = canvasGroup.LeanAlpha(0f, isPulsing ? 0f : duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }


}
