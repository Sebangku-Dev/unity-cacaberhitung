using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Parent class for all level gameplay class which can be inherited from
/// </summary>
public class BaseGameplay : Singleton<BaseGameplay>
{
    [Header("Base")]
    [SerializeField] protected Level levelData;
    [Header("Cutscene")]
    [SerializeField] protected Image cutsceneImage;
    [Header("Passed")]
    [SerializeField] protected Star starIsSolved;
    [SerializeField] protected Star starIsRightInTime;
    [SerializeField] protected Star starIsNoMistake;
    [SerializeField] protected UnityEvent OnPassed;
    [Header("Fail")]
    [SerializeField] protected UnityEvent OnFail;
    [Header("Ended")]
    [SerializeField] protected UnityEvent OnEnded;
    [SerializeField] protected LevelEndedModal modalEnded;

    public static event Action<LevelState> OnBeforeLevelStateChanged;

    public enum LevelState
    {
        Initialization,
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
        OnBeforeLevelStateChanged?.Invoke(newState);

        CurrentLevelState = newState;

        switch (newState)
        {
            case LevelState.Initialization:
                HandleInitialization();
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
    /// Invoked on start to init everything before prepare
    /// </summary>
    protected virtual void HandleInitialization() { }

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