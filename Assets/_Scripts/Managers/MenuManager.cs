using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private GameObject loaderCanvas;

    public void ActivateLoaderCanvas() => loaderCanvas?.SetActive(true);
    public void DeactivateLoaderCanvas() => loaderCanvas?.SetActive(false);
}
