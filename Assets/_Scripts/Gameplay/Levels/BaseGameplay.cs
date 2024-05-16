using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Parent class for all level gameplay class which can be inherited from
/// </summary>
public class BaseGameplay : Singleton<BaseGameplay>
{
    [Header("Base")]
    [SerializeField] public Level levelData;

    public static event Action<LevelState> OnStateChanged;

    public enum LevelState
    {
        Cutscene,
        Prepare,
        UserInteraction,
        Paused,
        Passed,
        Fail,
        Ended
    }
    public LevelState CurrentLevelState { get; private set; }

    public void ChangeState(LevelState newState)
    {
        CurrentLevelState = newState;

        switch (newState)
        {
            case LevelState.Cutscene:
                HandleCutscene();
                break;
            case LevelState.Prepare:
                HandlePrepare();
                break;
            case LevelState.UserInteraction:
                HandleUserInteraction();
                break;
            case LevelState.Paused:
                HandlePaused();
                break;
            case LevelState.Passed:
                HandlePassed();
                break;
            case LevelState.Fail:
                HandleFail();
                break;
            case LevelState.Ended:
                HandleEnded();
                break;
        }

        OnStateChanged?.Invoke(newState);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    #region Level State
    /// <summary>
    /// Invoked when paused
    /// </summary>
    protected virtual void HandlePaused() { }

    /// <summary>
    /// Invoked on start to play cutscene
    /// </summary>
    protected virtual void HandleCutscene() { }

    /// <summary>
    /// Invoked on very end state
    /// </summary>
    protected virtual void HandleEnded() { }

    /// <summary>
    /// Invoked if user give wrong answer
    /// </summary>
    protected virtual void HandleFail() { }

    /// <summary>
    /// Invoked if user give correct answer
    /// </summary>
    protected virtual void HandlePassed() { }

    /// <summary>
    /// Invoked after HandlePrepare()
    /// </summary>
    protected virtual void HandleUserInteraction() { }

    /// <summary>
    /// Invoked on first level load and after passed or fail state
    /// </summary>
    protected virtual void HandlePrepare() { }
    #endregion

    #region Utilities
    #endregion
}