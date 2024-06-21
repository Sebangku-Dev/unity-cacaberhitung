
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class MenuLevelModal : Modal
{
    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private UnityEvent OnReplay;
    [SerializeField] private UnityEvent OnResume;

    private bool isResumeInvoked;

    private void OnEnable()
    {
        BaseGameplay.Instance.ChangeState(BaseGameplay.LevelState.Paused);
    }
    private void OnDisable()
    {
        if (isResumeInvoked)
        {
            BaseGameplay.Instance.ChangeState(BaseGameplay.LevelState.UserInteraction);
            isResumeInvoked = false;
        }
    }

    public void ActivateMenuLevelModal() => base.ActivateModal();
    public void DeactivateMenuLevelModal() => base.DeactivateModal();
    public void Replay()
    {
        // No need to change state because it must be handled properly in level gameplay OnReplayClick
        SendMessage("OnDisable", true);

        OnReplay?.Invoke();
    }
    public void Resume()
    {
        isResumeInvoked = true;
        OnResume?.Invoke();
    }
    public void Exit()
    {
        OnExit?.Invoke();
    }

    public void Back()
    {
        NavigationSystem.Instance.Back();
    }
}
