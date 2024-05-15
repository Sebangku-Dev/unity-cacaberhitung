using UnityEngine;

public class Modal : MonoBehaviour
{
    [SerializeField] private string modalName;
    [SerializeField] Overlay relatedOverlay;

    public virtual void ActivateModal()
    {
        gameObject.SetActive(true);
        relatedOverlay.gameObject.SetActive(true);
    }
    
    public virtual void DeactivateModal()
    {
        gameObject.SetActive(false);
        relatedOverlay.gameObject.SetActive(false);
    }
}
