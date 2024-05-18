
using UnityEngine;

public class FadeAnimation : BaseAnimation
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        if (isAnimateOnLoad) Load();
    }

    public void Load()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        canvasGroup.LeanAlpha(0.6f, duration);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        transform.localScale = Vector3.one;
        int id = LeanTween.alpha(gameObject, 0f, duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }


}
