using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SizeToScale { x, y, z }
public class SizeMeasure : BaseAnimation
{
    [Header("SizeScale")]
    [SerializeField] SizeToScale scaleTarget;
    float targetScale;

    // Start is called before the first frame update
    void Start()
    {
        if (isAnimateOnLoad) Load();
    }

    public void Load()
    {
        gameObject.SetActive(true);

        targetScale = scaleTarget == SizeToScale.x ? gameObject.transform.localScale.x : scaleTarget == SizeToScale.y ? gameObject.transform.localScale.y : gameObject.transform.localScale.z;

        Vector3 initScale = new Vector3(
            gameObject.transform.localScale.x * (scaleTarget == SizeToScale.x ? 0.0f : 1.0f),
            gameObject.transform.localScale.y * (scaleTarget == SizeToScale.y ? 0.0f : 1.0f),
            gameObject.transform.localScale.z * (scaleTarget == SizeToScale.z ? 0.0f : 1.0f));

        gameObject.transform.localScale = initScale;

        switch (scaleTarget)
        {
            case SizeToScale.x:
                LeanTween.scaleX(gameObject, targetScale, duration).setEaseInOutQuart().setDelay(delay);
                break;
            case SizeToScale.y:
                LeanTween.scaleY(gameObject, targetScale, duration).setEaseInOutQuart().setDelay(delay);
                break;
            case SizeToScale.z:
                LeanTween.scaleZ(gameObject, targetScale, duration).setEaseInOutQuart().setDelay(delay);
                break;
            default:
                break;
        }

        if (isAnimateOnClose)
        {
            Invoke(nameof(Close), delayAfterOnLoad);
        }
    }

    public void Close()
    {
        int id = -1;

        switch (scaleTarget)
        {
            case SizeToScale.x:
                id = LeanTween.scaleX(gameObject, 0.0f, duration).id;
                break;
            case SizeToScale.y:
                id = LeanTween.scaleY(gameObject, 0.0f, duration).id;
                break;
            case SizeToScale.z:
                id = LeanTween.scaleZ(gameObject, 0.0f, duration).id;
                break;
            default:
                break;
        }

        LTDescr d = LeanTween.descr(id);

        d?.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
    }
}
