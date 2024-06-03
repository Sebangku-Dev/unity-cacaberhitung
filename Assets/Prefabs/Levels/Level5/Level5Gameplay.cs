using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class Level5Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public LevelSprite questionSprite;
    }

    [System.Serializable]
    public enum CakeType
    {
        Small, Big
    }

    [Header("Level3")]
    [SerializeField] private List<Questions> questions;


    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] Answer</para>
    /// <para>[2] Hint Text</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 0;
    private LevelSprite currentQuestionSprite;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        OnAfterLevelStateChanged += OnAfterStateChanged;
    }

    private void Start()
    {
        ChangeState(LevelState.Initialization);
    }

    protected override void Update()
    {
        Timer();
        WatchStar();
    }

    private void OnDestroy()
    {
        OnBeforeLevelStateChanged -= OnBeforeStateChanged;
        OnAfterLevelStateChanged -= OnAfterStateChanged;
    }
    #endregion

    #region Level State
    protected override async void HandleInitialization()
    {
        base.HandleInitialization();

        await PlayCutscene();

        StopTimer();
        CheckIsFirstPlay();
        SaveScoreState();
    }

    protected override async void HandlePrepare()
    {
        base.HandlePrepare();

        GenerateQuestion();
        ShowSprite(currentQuestionSprite);


        await Task.WhenAll(new Task[] {
            Task.Delay(2000)
            }
        );

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        // Prepare Timer
        StartTimer();

        ChangeState(LevelState.UserInteraction);
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();

        // Start timer if not active
        if (!isTimerActive) StartTimer();
    }

    protected override void HandlePaused()
    {
        base.HandlePaused();

        StopTimer();
    }

    protected override async void HandlePassed()
    {
        base.HandlePassed();

        HideSprite(currentQuestionSprite);

        OnPassed?.Invoke();

        // Wait for passed animation
        int duration = 1000;
        await Task.WhenAll(
            new Task[] { Task.Delay(duration) }
        );

        // Next question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            currentQuestionIndex++;
            ChangeState(LevelState.Prepare);
        }
        else
            ChangeState(LevelState.Ended);
    }

    protected override void HandleFail()
    {
        base.HandleFail();

        // Increase mistake
        mistake++;
        levelData.isNoMistake = false;

        // Event to trigger something e.g. Caca popup animation
        OnFail?.Invoke();

        // Reanswer user interaction
        ChangeState(LevelState.UserInteraction);
    }

    protected override async void HandleEnded()
    {
        base.HandleEnded();

        // No need to trigger hidesprites again because it overrided by its animation
        StopTimer();

        OnEnded?.Invoke();

        CalculateStars();
        levelData.playCount++;

        // Add score to user current score based on true-ish boolean
        AddScore((new bool[] { levelData.isSolved, levelData.isRightInTime, levelData.isNoMistake }).Where(c => c).Count());

        ShowAndUnlockNextLevel();
        await ShowEndedModal();
    }

    private void OnBeforeStateChanged(LevelState changedState)
    {

    }

    private void OnAfterStateChanged(LevelState changedState)
    {

    }
    #endregion

    #region Utilities
    private void GenerateQuestion()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSprite = questions[currentQuestionIndex].questionSprite;
    }
    #endregion

}
