using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Questions
{
    public string questionString;
    public GameObject questionSprite;
}

public class Level2Gameplay : BaseGameplay
{
    [Header("Base")]
    [SerializeField] private Level levelData;
    [Header("Cutscene")]
    [SerializeField] private Image cutsceneImage;
    [Header("Prepare")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TextMeshProUGUI clue2Text;
    [SerializeField] private TextMeshProUGUI answerOptionText;
    [SerializeField] private TextMeshProUGUI answer2OptionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [Header("Passed")]
    [SerializeField] private GameObject[] passedSprites;
    [Header("Fail")]
    [SerializeField] private GameObject[] failedSprites;
    [Header("Ended")]
    [SerializeField] private GameObject[] endedSprites;
    [SerializeField] private Modal modalEnded;

    private float cutsceneDuration = 2f;
    private float animationDuration = 0.5f;

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
    protected override async void HandleCutscene()
    {
        base.HandleCutscene();
        StopTimer();
        await PlayCutscene();
    }

    protected override void HandlePrepare()
    {
        base.HandlePrepare();

        GenerateQuestion();
        ShowQuestionSprites();
        ShowQuestionUI();
        StartTimer();
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();
    }

    protected override async void HandlePassed()
    {
        base.HandlePassed();

        await ShowSprites(passedSprites);
        HideQuestionSprite();

        // Next question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            currentQuestionIndex++;
            ChangeState(LevelState.Prepare);
        }
        else ChangeState(LevelState.Ended);

    }
    protected override async void HandleFail()
    {
        base.HandleFail();

        await ShowSprites(failedSprites);
        mistake++;

        // Reanswer the question
        if (currentQuestionIndex < questions.Count() - 1)
        {
            ChangeState(LevelState.UserInteraction);
        }
        else ChangeState(LevelState.Ended);
    }

    protected override async void HandleEnded()
    {
        base.HandleEnded();

        await ShowSprites(endedSprites, 3f);
        await ShowModal();
        // HideQuestionSprite();
        // HideQuestionUI();
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
        if (IsAnswerCorrect(buttonText.text, currentQuestion[3]))
            ChangeState(LevelState.Passed);
        else
            ChangeState(LevelState.Fail);
    }
    #endregion

    #region UI
    private void ShowQuestionUI()
    {
        ChangeButtonColor();

        questionText.transform.parent.gameObject.SetActive(true);
        answerOptionText.transform.parent.parent.gameObject.SetActive(true);
    }
    private void HideQuestionUI()
    {
        questionText.transform.parent.gameObject.SetActive(false);
        answerOptionText.transform.parent.parent.gameObject.SetActive(false);
    }

    private void ChangeButtonColor()
    {
        if (answerOptionText.text == "Benar")
        {
            answerOptionText.transform.parent.gameObject.GetComponent<Image>().color = new Color(40 / 255f, 206 / 255f, 156 / 255f);
            answerOptionText.transform.parent.gameObject.GetComponent<Shadow>().effectColor = new Color(20 / 255f, 186 / 255f, 136 / 255f);
        }
        if (answerOptionText.text == "Merah")
        {
            answerOptionText.transform.parent.gameObject.GetComponent<Image>().color = new Color(239 / 255f, 117 / 255f, 117 / 255f);
            answerOptionText.transform.parent.gameObject.GetComponent<Shadow>().effectColor = new Color(219 / 255f, 97 / 255f, 97 / 255f);
        }
        if (answer2OptionText.text == "Salah")
        {
            answer2OptionText.transform.parent.gameObject.GetComponent<Image>().color = new Color(239 / 255f, 117 / 255f, 117 / 255f);
            answer2OptionText.transform.parent.gameObject.GetComponent<Shadow>().effectColor = new Color(219 / 255f, 97 / 255f, 97 / 255f);
        }
        if (answer2OptionText.text == "Biru")
        {
            answer2OptionText.transform.parent.gameObject.GetComponent<Image>().color = new Color(121 / 255f, 154 / 255f, 238 / 255f);
            answer2OptionText.transform.parent.gameObject.GetComponent<Shadow>().effectColor = new Color(101 / 255f, 134 / 255f, 218 / 255f);
        }
    }

    private void ShowQuestionSprites()
    {
        currentQuestionSprite.SetActive(true);
    }

    private void HideQuestionSprite()
    {
        currentQuestionSprite.SetActive(false);
    }


    private async Task ShowSprites(GameObject[] blinks, float duration = 0f)
    {
        if (blinks != null && blinks.Count() > 1)
        {
            foreach (var blink in blinks)
            {
                blink.SetActive(true);
            }
        }
        else
        {
            blinks[0].SetActive(true);
        }

        // Disabling answer button for a while
        answerOptionText.transform.parent.GetComponent<Button>().interactable = false;
        answer2OptionText.transform.parent.GetComponent<Button>().interactable = false;

        // yield return new WaitForSeconds(animationDuration + duration);
        await Task.Delay(Mathf.RoundToInt(animationDuration * 1000 + duration * 1000));

        if (blinks != null && blinks.Count() > 1)
        {
            foreach (var blink in blinks)
            {
                blink.SetActive(false);
            }
        }
        else
        {
            blinks[0].SetActive(false);
        }

        // Re-enabling the button again
        answerOptionText.transform.parent.GetComponent<Button>().interactable = true;
        answer2OptionText.transform.parent.GetComponent<Button>().interactable = true;
    }

    private async Task ShowModal()
    {
        modalEnded.ActivateThis();
        await Task.Yield();
    }

    #endregion

    #region Utilities


    /// <summary>
    /// Format:
    /// [0] -> Question,
    /// [1] -> Option 1,
    /// [2] -> Option 2,
    /// [3] -> Valid Answer,
    /// [4] -> Clue 1,
    /// [5] -> Clue 2
    /// </summary>
    private List<string> currentQuestion;
    private GameObject currentQuestionSprite;
    private int currentQuestionIndex, mistake = 0;
    private float currentTime = 0;
    private bool isTimerActive = false;

    private void GenerateQuestion()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSprite = questions[currentQuestionIndex].questionSprite;

        questionText.text = $"{currentQuestion[0]}";
        answerOptionText.text = $"{currentQuestion[1]}";
        answer2OptionText.text = $"{currentQuestion[2]}";

        // Check for overloading values
        if (currentQuestion.Count > 4)
        {
            clueText.text = $"{currentQuestion[4]}";
            clue2Text.text = $"{currentQuestion[5]}";
        }
    }

    private bool IsAnswerCorrect(string answer, string valid) => answer == valid;
    private void StartTimer() => isTimerActive = true;
    private void StopTimer() => isTimerActive = false;
    private void Timer()
    {
        if (isTimerActive)
            currentTime += Time.deltaTime;

        timerText.text = Mathf.RoundToInt(currentTime).ToString();
    }

    private async Task PlayCutscene()
    {
        cutsceneImage.gameObject.SetActive(true);
        cutsceneImage.sprite = levelData.levelSprite;

        // yield return new WaitForSeconds(cutsceneDuration); // put your video here
        await Task.Delay(Mathf.RoundToInt(cutsceneDuration * 1000));

        cutsceneImage.gameObject.SetActive(false);
        ChangeState(LevelState.Prepare);
    }
    #endregion
}



