using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level8Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
    }

    [Header("Level8")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private LevelSprite questionPanel;
    [SerializeField] private List<LevelSprite> answerPanels;

    [Header("Hint")]
    [SerializeField] private RevealAnimation cacaHint;
    [SerializeField] private PopupAnimation panelHint;
    [SerializeField] private FadeAnimation overlayHint;

    /// <summary>
    /// Format:
    /// <para>[0] Question 1</para> 
    /// <para>[1] Question 2</para> 
    /// <para>[2] Option 1</para>
    /// <para>[3] Option 2</para>
    /// <para>[4] Option 3</para>
    /// <para>[5] Option 4</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 0;


    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
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

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        // Display First Hint
        if (currentQuestionIndex == 0)
        {
            cacaHint.Load();
            panelHint.Load();
            overlayHint.Load();

            // Wait for all running animation
            await Task.Delay(5000);
        }

        // Prepare the quiz questions
        GenerateQuestionAndSprite();

        // Wait for question and answers animation
        await LockAnswers(1000);

        // Prepare the timer
        StartTimer();

        ChangeState(LevelState.UserInteraction);
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();

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

        HideSprite(questionPanel);
        foreach (var answerPanel in answerPanels) HideSprite(answerPanel);

        await Task.WhenAll(new Task[] { LockAnswers(2000) });

        OnPassed?.Invoke();

        // Wait for a sec and delay answer to prevent user to access it

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

    protected override void HandleEnded()
    {
        base.HandleEnded();
    }

    private void OnBeforeStateChanged(LevelState changedState)
    {
    }
    #endregion

    #region User Interaction to Canvas
    public void OnAnswerClick(TextMeshProUGUI buttonText)
    {
        if (IsAnswerCorrect(buttonText.text))
            ChangeState(LevelState.Passed);
        else
            ChangeState(LevelState.Fail);
    }

    #endregion

    #region UI

    /// <summary>
    /// Method to lock answers from interactable
    /// </summary>
    /// <param name="delayMs">Delay in ms</param>
    /// <returns></returns>
    private async Task LockAnswers(int delayMs)
    {
        foreach (var answerPanel in answerPanels)
        {
            answerPanel.GetComponent<Button>().interactable = false;
            answerPanel.GetComponent<CanvasGroup>().alpha = 0.5f;
        }

        await Task.Delay(Mathf.RoundToInt(delayMs));

        foreach (var answerPanel in answerPanels)
        {
            answerPanel.GetComponent<Button>().interactable = true;
            answerPanel.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }


    #endregion

    #region Utilities
    private void GenerateQuestionAndSprite()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();

        // Set Question and Answer + Show Panels
        questionPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentQuestion[0]} x {currentQuestion[1]}";
        ShowSprite(questionPanel);

        for (int i = 0; i < answerPanels.Count; i++)
        {
            answerPanels[i].GetComponentInChildren<TextMeshProUGUI>().text = $"{currentQuestion[i + 2]}";
            ShowSprite(answerPanels[i]);
        }
    }

    protected bool IsAnswerCorrect(string answerText)
    {
        return Int16.Parse(answerText) == Int16.Parse(currentQuestion[0]) * Int16.Parse(currentQuestion[1]);
    }


    #endregion
}



