using UnityEngine;

public class Modal : MonoBehaviour
{
    [SerializeField] private string modalName;
    [SerializeField] Overlay relatedOverlay;

    public void ActivateThis()
    {
        gameObject.SetActive(true);
        relatedOverlay.gameObject.SetActive(true);
    }

    public void DeactivateThis()
    {
        gameObject.SetActive(false);
        relatedOverlay.gameObject.SetActive(false);
    }
}
