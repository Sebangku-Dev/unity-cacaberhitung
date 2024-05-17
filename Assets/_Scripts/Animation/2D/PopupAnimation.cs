using System.Threading.Tasks;
using UnityEngine;

public class PopupAnimation : BaseAnimation
{
    private void Start()
    {
        if (isAnimateOnLoad) Load();
    }


    public async void Load()
    {
        gameObject.SetActive(true);

        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, duration).setEaseInOutQuart();

        if (isAnimateOnClose)
        {
            await Task.Delay(Mathf.RoundToInt(delayAfterOnLoad) * 1000);
            await Close();
        }
    }

    public async Task Close()
    {
        transform.localScale = Vector3.one;
        int id = LeanTween.scale(gameObject, Vector3.zero, duration - 0.1f).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }

        await Task.Delay(300);
    }
}
