using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class ExampleLevelGameplay : BaseGameplay
{
    [SerializeField] private Level level;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private int maxTimeDuration;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        BaseGameplay.OnStateChanged += OnLevelStateChanged;
    }
    private void Start()
    {
        ChangeState(LevelState.Prepare);
    }
    private void Update()
    {
        Timer();
    }

    private void OnDestroy()
    {
        BaseGameplay.OnStateChanged -= OnLevelStateChanged;
    }
    #endregion

    #region Level State
    protected override void HandlePrepare()
    {
        base.HandlePrepare();

        GenerateQuestion();
        StartTimer();
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();
    }

    protected override void HandlePassed()
    {
        base.HandlePassed();

        Debug.Log("Pintar");

        // Next question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            currentQuestionIndex++;
            ChangeState(LevelState.Prepare);
        }
        else ChangeState(LevelState.Ended);

    }
    protected override void HandleFail()
    {
        base.HandleFail();

        Debug.Log("Belajar lagi ya");
        mistake++;

        // Reanswer the question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            ChangeState(LevelState.UserInteraction);
        }
        else ChangeState(LevelState.Ended);
    }

    protected override void HandleEnded()
    {
        base.HandleEnded();

        StopTimer();

        // Booleans check
        level.isSolved = true;

        if (mistake > 0)
        {
            level.isNoMistake = false;
        }
        else level.isNoMistake = true;

        if (currentTime <= maxTimeDuration)
        {
            level.isRightInTime = true;
        }
        else level.isRightInTime = false;
    }

    private void OnLevelStateChanged(LevelState changedState)
    {
        // Do in every state changes
    }
    #endregion

    #region User Interaction to Canvas
    public void OnAnswerClick(TextMeshProUGUI buttonText)
    {
        if (IsAnswerCorrect(buttonText.text, currentQuestion[2], currentQuestion[3]))
        {
            ChangeState(LevelState.Passed);
        }
        else
        {
            ChangeState(LevelState.Fail);
        }
    }
    #endregion

    #region Utilities

    // format -> question1;question2;operator;validAnswer
    private List<string> questions = new List<string>()
    {
        "9;8;>;>",
        "1;2;>;<",
        "10;8;>;>",
        "9;3;<;>",
        "2;8;>;<",
    };

    private List<string> currentQuestion;
    private int currentQuestionIndex, mistake = 0;
    private float currentTime = 0;
    private bool isTimerActive = false;

    private void GenerateQuestion()
    {
        this.currentQuestion = questions[currentQuestionIndex].Split(";").ToList();
        questionText.text = $"{currentQuestion[0]} {currentQuestion[2]} {currentQuestion[1]} = ?";
    }

    private bool IsAnswerCorrect(string answer, string opt, string valid) => (opt == valid && answer == "Benar") || (opt != valid && answer == "Salah");
    private void StartTimer() => isTimerActive = true;
    private void StopTimer() => isTimerActive = false;
    private void Timer()
    {
        if (isTimerActive)
            currentTime += Time.deltaTime;

        stateText.text = Mathf.RoundToInt(currentTime).ToString();
    }
    #endregion
}



