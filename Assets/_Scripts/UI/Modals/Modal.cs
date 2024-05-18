using UnityEngine;
using UnityEngine.Events;

public class Modal : MonoBehaviour
{
    [SerializeField] protected string modalName;

    [SerializeField] protected UnityEvent OnModalActivate;
    [SerializeField] protected UnityEvent OnModalDeactivate;

    protected virtual void ActivateModal()
    {
        OnModalActivate?.Invoke();
    }

    protected virtual void DeactivateModal()
    {
        OnModalDeactivate?.Invoke();
    }
}
