using System.Threading.Tasks;
using UnityEngine;

public class PopupAnimation : BaseAnimation
{
    private void Start()
    {
        if (isAnimateOnLoad) Load();
    }


    public void Load()
    {
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, duration).setEaseInOutQuart().setDelay(delay);

        if (isAnimateOnClose)
        {
            Invoke("Close", delayAfterOnLoad);
        }
    }

    public void Close()
    {
        if (transform) transform.localScale = Vector3.one;
        int id = LeanTween.scale(gameObject, Vector3.zero, duration - 0.1f).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }
    }
}
