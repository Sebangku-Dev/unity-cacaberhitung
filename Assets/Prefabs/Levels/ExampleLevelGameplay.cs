using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ExampleLevelGameplay : BaseGameplay
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI stateText;

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

        // Reanswer the question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            ChangeState(LevelState.UserInteraction);
        }
        else ChangeState(LevelState.Ended);
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
    private int currentQuestionIndex = 0;

    private void GenerateQuestion()
    {
        this.currentQuestion = questions[currentQuestionIndex].Split(";").ToList();
        questionText.text = $"{currentQuestion[0]} {currentQuestion[2]} {currentQuestion[1]} = ?";
    }

    private bool IsAnswerCorrect(string answer, string opt, string valid)
    {
        return (opt == valid && answer == "Benar") || (opt != valid && answer == "Salah");
    }
    #endregion
}



