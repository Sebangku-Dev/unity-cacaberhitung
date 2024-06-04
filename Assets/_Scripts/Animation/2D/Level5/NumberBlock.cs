using System;
using UnityEngine;

public class NumberBlock : BaseAnimation, IAnimate
{
    [Serializable]
    public enum Type
    {
        Hundred, Ten, One
    }

    [Header("Main")]
    [SerializeField] public Type numberBlockType;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    public void SetAlpha(float alphaAmount)
    {
        canvasGroup.LeanAlpha(alphaAmount, duration).setDelay(delay);
        Debug.Log(123);
    }

    public void Load()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;

        canvasGroup.LeanAlpha(1f, duration).setDelay(delay);
        transform.LeanScale(Vector3.one, duration).setEaseInOutQuart().setDelay(delay + 0.2f);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        transform.localScale = Vector3.one;
        int id = canvasGroup.LeanAlpha(0f, duration + 0.2f).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }
}
