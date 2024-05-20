
using UnityEngine;
using UnityEngine.Events;

public class MenuLevelModal : Modal
{
    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private UnityEvent OnReplay;
    [SerializeField] private UnityEvent OnResume;

    private void OnEnable()
    {
        BaseGameplay.Instance.ChangeState(BaseGameplay.LevelState.Paused);
    }

    private void OnDisable()
    {
        BaseGameplay.Instance.ChangeState(BaseGameplay.LevelState.UserInteraction);
    }

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
    }
}
