using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Parent class for all level gameplay class which can be inherited from
/// </summary>
public class BaseGameplay : Singleton<BaseGameplay>
{
    public static event Action<LevelState> OnStateChanged;

    public enum LevelState
    {
        Prepare,
        UserInteraction,
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
            case LevelState.Prepare:
                HandlePrepare();
                break;
            case LevelState.UserInteraction:
                HandleUserInteraction();
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
        Debug.Log("invoked");
    }

    protected override void Awake()
    {
        base.Awake();
        // Implemented on child class
    }

    /// <summary>
    /// Invoked on very end state
    /// </summary>
    protected virtual void HandleEnded()
    {
        // Implemented on child class
    }

    /// <summary>
    /// Invoked if user give wrong answer
    /// </summary>
    protected virtual void HandleFail()
    {
        // Implemented on child class
    }

    /// <summary>
    /// Invoked if user give correct answer
    /// </summary>
    protected virtual void HandlePassed()
    {
        // Implemented on child class
    }

    /// <summary>
    /// Invoked after HandlePrepare()
    /// </summary>
    protected virtual void HandleUserInteraction()
    {
        // Implemented on child class
    }

    /// <summary>
    /// Invoked on first level load and after passed or fail state
    /// </summary>
    protected virtual void HandlePrepare()
    {
        // Implemented on child class
    }
}