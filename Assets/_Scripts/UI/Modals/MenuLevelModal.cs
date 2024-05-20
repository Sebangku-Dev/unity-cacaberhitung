using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuLevelModal : Modal
{
    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private UnityEvent OnReplay;
    [SerializeField] private UnityEvent OnResume;

    public void ActivateMenuLevelModal() => base.ActivateModal();
    public void DeactivateMenuLevelModal() => base.DeactivateModal();
    public void Replay()
    {
        OnReplay?.Invoke();
    }
    public void Resume()
    {
        OnResume?.Invoke();
    }
    public void Exit()
    {
        OnExit?.Invoke();
        ScoreSystem.Instance.AddScore(3);
    }
}
