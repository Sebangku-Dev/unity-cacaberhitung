using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Questions
{
    public string questionString;
    public Sprite questionSprite;
}

public class Level2Gameplay : BaseGameplay
{

    [Header("Data")]
    [SerializeField] private Level levelData;
    [SerializeField] private List<Questions> questions;

    [Header("Assets")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answerOptionText;
    [SerializeField] private TextMeshProUGUI answer2OptionText;
    [SerializeField] private GameObject questionAnswerGameObject;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image cutsceneImage;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        BaseGameplay.OnStateChanged += OnLevelStateChanged;
    }
    private void Start()
    {
        ChangeState(LevelState.Cutscene);
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
    protected override void HandleCutscene()
    {
        base.HandleCutscene();
        StopTimer();
        StartCoroutine(PlayCutscene());
    }

    protected override void HandlePrepare()
    {
        base.HandlePrepare();

        GenerateQuestion();
        ShowQuestionUI();
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
        levelData.isSolved = true;

        if (mistake > 0)
        {
            levelData.isNoMistake = false;
        }
        else levelData.isNoMistake = true;

        if (currentTime <= levelData.maxTimeDuration)
        {
            levelData.isRightInTime = true;
        }
        else levelData.isRightInTime = false;
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
    /// <summary>
    /// Format:
    /// [0] -> Question,
    /// [1] -> Option 1,
    /// [2] -> Option 2,
    /// [3] -> Valid Answer,
    /// [4] -> Clue 1,
    /// [5] -> Clue 2
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>


    private List<string> currentQuestion;
    private int currentQuestionIndex, mistake = 0;
    private float currentTime = 0;
    private bool isTimerActive = false;

    private void GenerateQuestion()
    {
        // this.currentQuestion = questions[currentQuestionIndex].Split(";").ToList();
        questionText.text = $"{currentQuestion[0]}";
        answerOptionText.text = $"{currentQuestion[1]}";
        answer2OptionText.text = $"{currentQuestion[2]}";
    }

    private void ShowQuestionUI()
    {
        // You can implement some animation(s) here
        questionAnswerGameObject.SetActive(true);
    }

    private void ShowQuestionSprites()
    {

    }

    private void HideQuestionUI()
    {
        questionAnswerGameObject.SetActive(false);
    }

    private bool IsAnswerCorrect(string answer, string opt, string valid) => (opt == valid && answer == "Benar") || (opt != valid && answer == "Salah");
    private void StartTimer() => isTimerActive = true;
    private void StopTimer() => isTimerActive = false;
    private void Timer()
    {
        if (isTimerActive)
            currentTime += Time.deltaTime;

        timerText.text = Mathf.RoundToInt(currentTime).ToString();
    }

    private IEnumerator PlayCutscene()
    {
        cutsceneImage.gameObject.SetActive(true);
        cutsceneImage.sprite = levelData.levelSprite;

        yield return new WaitForSeconds(5); // put your video here

        cutsceneImage.gameObject.SetActive(false);
        ChangeState(LevelState.Prepare);
    }
    #endregion
}



