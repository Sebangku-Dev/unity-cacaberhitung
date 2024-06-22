
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        OnReplay?.Invoke();
        NavigationSystem.Instance.LoadScene(SceneManager.GetActiveScene().name, false);
    }
    public void Resume()
    {
        OnResume?.Invoke();
        isResumeInvoked = true;
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
