using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHintAnimation : BaseAnimation, IAnimate
{
    [Header("Main")]
    [SerializeField] public Transform child;
    [SerializeField] private Vector3 delta;

    private void Awake()
    {
        if (isAnimateOnLoad) Load();

        // Set initial state
        child.localScale = Vector3.zero;
        transform.LeanMoveLocal(transform.localPosition + delta, 0f);
    }

    public void Load()
    {
        gameObject.SetActive(true);

        transform.LeanMoveLocal(transform.localPosition - delta, duration).setEaseInOutQuart().setDelay(delay);
        child.LeanScale(Vector3.one, duration).setEaseInOutQuart().setDelay(duration + delay);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }
    public void Close()
    {
        child.LeanScale(Vector3.zero, duration).setEaseInOutQuart();
        
        int id = transform.LeanMoveLocal(transform.localPosition + delta, duration).setEaseInOutQuart().setDelay(duration).id;
        LTDescr d = LeanTween.descr(id);


        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart);
        }

    }
}
