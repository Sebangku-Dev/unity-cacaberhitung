using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Questions
{
    public string questionString;
    public LevelSprite questionSprite;
}

public class Level2Gameplay : BaseGameplay
{

    [Header("Level2")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private TextMeshProUGUI clue2Text;
    [SerializeField] private TextMeshProUGUI answerOptionText;
    [SerializeField] private TextMeshProUGUI answer2OptionText;


    private float cutsceneDuration = 2f;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        BaseGameplay.OnBeforeLevelStateChanged += OnBeforeStateChanged;
    }
    private void Start()
    {
        CheckIsFirstPlay();
        ChangeState(LevelState.Cutscene);
    }
    private void Update()
    {
        Timer();
        WatchStar();
    }

    private void OnDestroy()
    {
        BaseGameplay.OnBeforeLevelStateChanged -= OnBeforeStateChanged;
    }
    #endregion

    #region Level State
    protected override async void HandleCutscene()
    {
        base.HandleCutscene();
        StopTimer();
        await PlayCutscene();
    }

    protected override async void HandlePrepare()
    {
        base.HandlePrepare();

        // Prepare the quiz questions
        GenerateQuestion();
        ShowSprite(currentQuestionSprite);
        ShowQuestionUI();
        await DelayAnswer(4000f);

        // Prepare the timer
        StartTimer();

        ChangeState(LevelState.UserInteraction);
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();
    }

    protected override void HandlePassed()
    {
        base.HandlePassed();

        HideSprite(currentQuestionSprite);
        OnPassed?.Invoke();

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

        // No need to trigger hidesprites again because it overrided by its animation
        mistake++;
        levelData.isNoMistake = false;

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

        // Gained star re-checking
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

        // Increase play count
        levelData.playCount++;

        // Show modal
        await ShowModal();
    }

    private void OnBeforeStateChanged(LevelState changedState)
    {
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
    private void ShowSprite(LevelSprite sprite) => sprite.Load();
    private void HideSprite(LevelSprite sprite) => sprite.Close();


    private async Task ShowModal()
    {
        modalEnded.ActivateEndedModal(levelData.isSolved, levelData.isRightInTime, levelData.isNoMistake);
        await Task.Yield();
    }

    private void WatchStar()
    {
        starIsSolved.gameObject.SetActive(levelData.isSolved);
        // Animate time star like a stopwatch
        starIsRightInTime.gameObject.GetComponent<Image>().fillAmount = 1 - (currentTime / levelData.maxTimeDuration);
        starIsNoMistake.gameObject.SetActive(levelData.isNoMistake);

    }

    private async Task DelayAnswer(float interStateDelay)
    {
        answerOptionText.transform.parent.GetComponent<Button>().interactable = false;
        answer2OptionText.transform.parent.GetComponent<Button>().interactable = false;

        await Task.Delay(Mathf.RoundToInt(interStateDelay));

        answerOptionText.transform.parent.GetComponent<Button>().interactable = true;
        answer2OptionText.transform.parent.GetComponent<Button>().interactable = true;
    }


    #endregion

    #region Utilities
    /// <summary>
    /// Format:
    /// [0] Question
    /// [1] Option 1
    /// [2] Option 2
    /// [3] Valid Answer
    /// [4] Clue 1
    /// [5] Clue 2
    /// </summary>
    private List<string> currentQuestion;
    private LevelSprite currentQuestionSprite;
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

        // Star checking
        if (currentTime <= levelData.maxTimeDuration)
        {
            levelData.isRightInTime = false;
        }
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

    private void CheckIsFirstPlay()
    {
        if (levelData.playCount < 1)
        {
            levelData.isSolved = true;
            levelData.isRightInTime = true;
            levelData.isNoMistake = true;
        }
    }
    #endregion
}



