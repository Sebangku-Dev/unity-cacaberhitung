using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArithmethicBlock : BaseAnimation, IAnimate
{
    [Header("Main")]

    [SerializeField] private CanvasGroup canvasGroup;

    public void SetAlpha(float alphaAmount)
    {
        canvasGroup.LeanAlpha(alphaAmount, duration).setDelay(delay);
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
