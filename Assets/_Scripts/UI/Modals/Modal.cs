using UnityEngine;
using UnityEngine.Events;

public class Modal : MonoBehaviour
{
    [SerializeField] private string modalName;

    [SerializeField] private UnityEvent OnModalActivate;
    [SerializeField] private UnityEvent OnModalDeactivate;

    public virtual void ActivateModal()
    {
        OnModalActivate?.Invoke();
    }

    public virtual void DeactivateModal()
    {
        OnModalDeactivate?.Invoke();
    }
}
