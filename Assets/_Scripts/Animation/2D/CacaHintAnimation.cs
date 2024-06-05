using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacaHintAnimation : BaseAnimation, IAnimate
{
    [Header("Main")]
    [SerializeField] GameObject panelQuestion;

    private void Awake()
    {
        // Set initial state
        panelQuestion.transform.localScale = Vector3.zero;
        transform.LeanMoveLocalY(transform.localPosition.y + -500, 0f);
    }

    public void Load()
    {
        gameObject.SetActive(true);

        transform.LeanMoveLocalY(transform.localPosition.y + 500, duration).setEaseInOutQuart().setDelay(delay);
        panelQuestion.transform.LeanScale(Vector3.one, duration).setEaseInOutQuart().setDelay(duration + delay);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }
    public void Close()
    {
        int id = transform.LeanMoveLocalY(transform.localPosition.y + -500, duration).setEaseInOutQuart().setDelay(delay).id;
        LTDescr d = LeanTween.descr(id);

        panelQuestion.transform.LeanScale(Vector3.zero, duration).setEaseInOutQuart().setDelay(delay + duration);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }

    }

}
