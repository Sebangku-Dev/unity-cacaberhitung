using System;
using UnityEngine;

/// <summary>
/// Parent class for all level gameplay class which can be inherited from
/// </summary>
public class BaseGameplay : Singleton<BaseGameplay>
{
    public static event Action<LevelState> OnStateChanged;

    public enum LevelState
    {
        Prepare = 0,
        UserInteraction = 1,
        Passed = 2,
        Fail = 3,
        Ended = 4
    }
    public LevelState CurrentLevelState;

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
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnStateChanged?.Invoke(newState);
    }

    protected virtual void HandleEnded()
    {
        // Implemented on child class
    }

    protected virtual void HandleFail()
    {
        // Implemented on child class
    }

    protected virtual void HandlePassed()
    {
        // Implemented on child class
    }

    protected virtual void HandleUserInteraction()
    {
        // Implemented on child class
    }

    protected virtual void HandlePrepare()
    {
        // Implemented on child class
    }
}