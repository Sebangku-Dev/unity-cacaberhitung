using UnityEngine;
using UnityEngine.Events;

public class LevelSprite : MonoBehaviour
{
    [SerializeField] private UnityEvent OnLoad;
    [SerializeField] private UnityEvent OnClose;

    public void Load()
    {
        OnLoad?.Invoke();
    }

    public void Close()
    {
        OnClose?.Invoke();
    }

}
