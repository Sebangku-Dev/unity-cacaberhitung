using UnityEngine;
using System.Threading.Tasks;

public class RevealAnimation : BaseAnimation
{
    [SerializeField] Vector3 startingLocation;
    [SerializeField] Direction direction;

    private Vector3 currentPosition;

    private void Start()
    {
        if (isAnimateOnLoad) Load();
    }

    public async void Load()
    {

        gameObject.SetActive(true);

        currentPosition = transform.position;
        transform.position = startingLocation;

        transform.LeanMoveLocalY(currentPosition.y, duration).setEaseOutExpo();

        if (isAnimateOnClose)
        {
            await Task.Delay(Mathf.RoundToInt(delayAfterOnLoad) * 1000);
            await Close();
        }
    }

    public async Task Close()
    {

        int id = LeanTween.moveLocalY(gameObject, startingLocation.y, duration).id;
        LTDescr d = LeanTween.descr(id);

        if (d != null)
        {
            d.setOnComplete(() => gameObject.SetActive(false)).setEase(LeanTweenType.easeInOutQuart).setDelay(delay);
        }

        await Task.Yield();
    }

}

[System.Serializable]
public enum Direction
{
    Left,
    Right,
    Top,
    Bottom
}
