using System.Threading.Tasks;
using UnityEngine;

public class BaseAnimation : MonoBehaviour
{
    [SerializeField] public float duration = 0.3f;
    [Header("Load")]
    [SerializeField] protected bool isAnimateOnLoad;
    [SerializeField] protected float delay = 0f;
    [Header("Close")]
    [SerializeField] protected bool isAnimateOnClose;
    [SerializeField] protected float delayAfterOnLoad = 2f;
}

[System.Serializable]
public enum Direction
{
    Left,
    Right,
    Top,
    Bottom
}

internal interface IAnimate
{
    void Load();
    void Close();
}

