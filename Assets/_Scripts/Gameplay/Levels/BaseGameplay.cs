using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// Parent class for all level gameplay class which can be inherited from
/// </summary>
public class BaseGameplay : Singleton<BaseGameplay>
{
    [Header("Base")]
    [SerializeField] protected Level levelData;
    [Header("Cutscene")]
    [SerializeField] protected Image whiteOverlay;
    [SerializeField] protected VideoPlayer cutscenePlayer;
    [Header("Prepare")]
    [SerializeField] protected UnityEvent OnPrepare;
    [Header("User Interaction")]
    [SerializeField] protected UnityEvent OnUserInteraction;
    [Header("Paused")]
    [SerializeField] protected UnityEvent OnPaused;
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
    public static event Action<LevelState> OnAfterLevelStateChanged;

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

        OnAfterLevelStateChanged?.Invoke(newState);
    }



    protected override void Awake()
    {
        base.Awake();

        // For smoother transition between loading and cutscene
        whiteOverlay.gameObject.SetActive(true);
    }

    protected virtual void Update()
    {

    }

    #region Level State

    /// <summary>
    /// Invoked on start to init everything before prepare
    /// </summary>
    protected virtual void HandleInitialization() { Debug.Log(LevelState.Initialization); }

    /// <summary>
    /// Invoked when on paused state
    /// </summary>
    protected virtual void HandlePaused() { Debug.Log(LevelState.Paused); }

    /// <summary>
    /// Invoked on very end state
    /// </summary>
    protected virtual async void HandleEnded()
    {
        Debug.Log(LevelState.Ended);

        // No need to trigger hidesprites again because it overrided by its animation
        StopTimer();

        OnEnded?.Invoke();

        // Calculate the stars
        CalculateStars();

        // Increase play count
        levelData.playCount++;

        // Show and unlock next level   
        ShowAndUnlockNextLevel();

        // Show ended modal
        await ShowEndedModal();

        // Add score to user current score based on true-ish boolean
        AddScore((new bool[] { levelData.isSolved, levelData.isRightInTime, levelData.isNoMistake }).Where(c => c).Count());

        AchievementManager.Instance.EvaluatePossibleAchievement();

        // Save user data -> score, etc
        UserManager.Instance.Save();

    }

    /// <summary>
    /// Invoked if user give wrong answer
    /// </summary>
    protected virtual void HandleFail() { Debug.Log(LevelState.Fail); }

    /// <summary>
    /// Invoked if user give correct answer
    /// </summary>
    protected virtual void HandlePassed() { Debug.Log(LevelState.Passed); }

    /// <summary>
    /// Invoked after HandlePrepare()
    /// </summary>
    protected virtual void HandleUserInteraction() { Debug.Log(LevelState.UserInteraction); }

    /// <summary>
    /// Invoked on first level load and after passed or fail state
    /// </summary>
    protected virtual void HandlePrepare() { Debug.Log(LevelState.Prepare); }
    #endregion

    #region UI
    /// <summary>
    /// Method to show LevelSprite. Dont forget to put LevelSprite class to gameObject that you want to show up
    /// </summary>
    /// <param name="sprite">Sprite which you want to show</param>
    protected void ShowSprite(LevelSprite sprite) => sprite.Load();

    /// <summary>
    /// Method to hide LevelSprite. Dont forget to put LevelSprite class to gameObject that you want to hide in 
    /// </summary>
    /// <param name="sprite">Sprite which you want to hide</param>
    protected void HideSprite(LevelSprite sprite) => sprite.Close();

    protected LevelSprite GenerateSprite(LevelSprite sprite, Transform parent, Vector3 initialPosition)
    {
        return Instantiate(sprite, initialPosition, Quaternion.identity, parent);
    }

    protected void DestroySprite(LevelSprite sprite) => Destroy(sprite.gameObject);

    /// <summary>
    /// Method to show EndedModal at <see cref="LevelState.Ended"/>
    /// </summary>
    /// <returns></returns>
    protected async Task ShowEndedModal()
    {
        modalEnded.ActivateEndedModal(levelData.isSolved, levelData.isRightInTime, levelData.isNoMistake);
        await Task.Yield();
    }
    #endregion

    #region Utilities
    protected int mistake = 0;
    protected float currentTime = 0;
    protected bool isTimerActive = false;
    protected void StartTimer() => isTimerActive = true;
    protected void StopTimer() => isTimerActive = false;
    protected void Timer()
    {
        if (isTimerActive)
            currentTime += Time.deltaTime;
    }

    /// <summary>
    /// Variable that holds cutscene duration
    /// </summary>
    protected float cutsceneDuration = 4.5f;

    /// <summary>
    /// Method to play cutscene. Strongly related to <see cref="cutscenePlayer"/> to play the <see cref="levelData.cutSceneClip"/>
    /// </summary>
    /// <returns>Task.Delay at specific <see cref="cutsceneDuration"/></returns>
    protected async Task PlayCutscene()
    {
        cutscenePlayer.clip = levelData.cutsceneClip;
        cutscenePlayer.transform.parent.gameObject.SetActive(true);

        // yield return new WaitForSeconds(cutsceneDuration); // put your video here
        await Task.Delay(Mathf.RoundToInt(cutsceneDuration * 1000));

        // Hide the cutscene
        cutscenePlayer.transform.parent.gameObject.SetActive(false);
        whiteOverlay.gameObject.SetActive(false);

        ChangeState(LevelState.Prepare);
    }

    /// <summary>
    /// Variable that holds temporary star state. Strongly related to <see cref="SaveScoreState"/>
    /// </summary>
    protected bool starIsSolvedState, starIsNoMistakeState, starIsRightInTimeState;

    /// <summary>
    /// If first play, then set all the stars to true. Related to stars panel
    /// </summary>
    protected void CheckIsFirstPlay()
    {
        if (levelData.playCount < 1)
        {
            levelData.isSolved = true;
            levelData.isRightInTime = true;
            levelData.isNoMistake = true;
        }
    }

    /// <summary>
    /// Used for OnReplay in Paused Modal to preserve the early star state before playing
    /// <para>Strongly related to <see cref="starIsSolvedState"/>, <see cref="starIsNoMistakeState"/>, <see cref="starIsRightInTimeState"/></para>
    /// </summary>
    protected void SaveScoreState()
    {
        starIsSolvedState = levelData.isSolved;
        starIsRightInTimeState = levelData.isRightInTime;
        starIsNoMistakeState = levelData.isNoMistake;
    }

    /// <summary>
    /// Method to interact with stars panel based on <see cref="levelData"/> boolean values. Strongly related to Stars gameObject
    /// </summary>
    protected void WatchStar()
    {
        starIsSolved.gameObject.SetActive(levelData.isSolved);
        starIsRightInTime.gameObject.GetComponent<Image>().fillAmount = 1 - (currentTime / levelData.maxTimeDuration);
        starIsNoMistake.gameObject.SetActive(levelData.isNoMistake);
    }

    /// <summary>
    /// Calculate the stars based on <see cref="levelData"/> value
    /// </summary>
    protected void CalculateStars()
    {
        levelData.isSolved = true;

        if (mistake > 0)
            levelData.isNoMistake = false;
        else
            levelData.isNoMistake = true;

        if (currentTime <= levelData.maxTimeDuration)
            levelData.isRightInTime = true;
        else
            levelData.isRightInTime = false;
    }

    /// <summary>
    /// <see cref="ScoreSystem.AddScore(int)"/> wrapper 
    /// </summary>
    /// <param name="addedScore">Added score in integer</param>
    protected void AddScore(int addedScore)
    {
        ScoreManager.Instance.AddScore(addedScore);
    }

    /// <summary>
    /// Used in <see cref="LevelState.Ended"/> to show and unlock next level by <see cref="levelData"/> value
    /// </summary>
    protected void ShowAndUnlockNextLevel()
    {
        levelData.isToBePlayed = false;

        if (levelData.nextLevel != null)
        {
            levelData.nextLevel.isToBePlayed = true;
            levelData.nextLevel.isUnlocked = true;
            levelData.nextLevel.isSolved = false;
        }
    }

    #endregion
}