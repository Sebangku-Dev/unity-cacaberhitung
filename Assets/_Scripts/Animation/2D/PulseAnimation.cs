using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseAnimation : BaseAnimation, IAnimate
{
    [Header("Pulse")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] float targetAlpha;

    private void OnEnable()
    {
        if (isAnimateOnLoad) Load();
    }

    public void Load()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;

        canvasGroup.LeanAlpha(targetAlpha > 0.0f ? targetAlpha : 0.6f, duration).setDelay(delay).setLoopPingPong();

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }


    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
