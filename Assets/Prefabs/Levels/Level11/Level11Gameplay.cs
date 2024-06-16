using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Level11Gameplay : BaseGameplay
{
    [Serializable]
    public class Questions
    {
        public string question;
        public int validAnswerIndex;
        public AnswerType answerType;
        public List<string> answers;
        public LevelSprite shape;
        public Transform[] pointToHits;
    }

    [Serializable]
    public enum AnswerType
    {
        Text, Image
    }

    [Header("Level 11")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private LevelSprite questionPanel;
    [SerializeField] private LevelSprite answerPanel;
    [SerializeField] private RevealAnimation caca, enemy;
    [SerializeField] private CharacterHintAnimation cacaHint;
    [SerializeField] private FadeAnimation overlay;
    [SerializeField] private DrawerController drawer;

    private string currentQuestion;

    /// <summary>
    /// If <see cref="AnswerType.Text"/> then it's the number text. If <see cref="AnswerType.Image"/> then it's the Resource path
    /// </summary>
    private List<string> currentAnswers;
    private int currentValidAnswerIndex;
    private AnswerType currentAnswerType;
    private LevelSprite currentQuestionShape;
    private Transform[] currentPointToHits;
    private int currentQuestionIndex = 0;

    private enum SubLevelState
    {
        Drawing, Answering
    }
    private SubLevelState currentSubLevelState;

    private void ChangeSubLevelState(SubLevelState changedState)
    {
        currentSubLevelState = changedState;

        switch (changedState)
        {
            case SubLevelState.Drawing:
                HandleDrawing();
                break;
            case SubLevelState.Answering:
                HandleAnswering();
                break;
        }
    }

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        DrawerController.OnPointerMoveEvent += OnDraw;
        DrawerController.OnPointerUpEvent += OnEndDraw;

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
        DrawerController.OnPointerMoveEvent -= OnDraw;
        DrawerController.OnPointerUpEvent -= OnEndDraw;
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

        // Disable drawer for a while
        drawer.gameObject.SetActive(false);

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        // Prepare the quiz questions
        PrepareQuestionAndSprite();

        // Display first animation and hint
        if (currentQuestionIndex == 0)
        {
            caca.Load();
            enemy.Load();

            // Wait for the animation
            await Task.Delay(1000);

            overlay.Load();
            cacaHint.Load();

            await Task.Delay(4000);
        }

        // Change to drwaing state
        ChangeSubLevelState(SubLevelState.Drawing);

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

        ResetAllQuestionAndSprite();
        // foreach (var answerPanel in answerPanels) HideSprite(answerPanel);

        OnPassed?.Invoke();

        // Reset all question and sprite

        await Task.Delay(1000);

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
    }

    private void OnBeforeStateChanged(LevelState changedState)
    {
    }
    #endregion

    #region Sub Level State

    private void HandleDrawing()
    {
        GenerateSprite();

        // Prepare the drawer and shape
        drawer.gameObject.SetActive(true);
        SetShapeAlpha(0f);
    }

    private void HandleAnswering()
    {
        drawer.gameObject.SetActive(false);
        GenerateQuestion();
    }

    #endregion

    #region User Interaction to Canvas

    [Header("UI Interaction")]
    [SerializeField] private Camera cam;
    private int currentIndexPointToHit = 0;

    private Color blueColor = new Color(121 / 255f, 154 / 255f, 238 / 255f);

    private void OnDraw()
    {
        if (currentIndexPointToHit == currentPointToHits.Count()) return;

        currentPointToHits[0].GetComponent<Image>().color = Color.cyan;
        SetShapeAlpha((float)currentIndexPointToHit / (float)(currentPointToHits.Count() - 1));

        if (Vector3.Distance(Input.mousePosition, (Vector3)RectTransformUtility.WorldToScreenPoint(cam, currentPointToHits[currentIndexPointToHit].position)) < 50f)
        {
            currentPointToHits[currentIndexPointToHit].gameObject.SetActive(false);
            currentIndexPointToHit++;
        }
    }

    private void OnEndDraw()
    {

        if (currentIndexPointToHit == currentPointToHits.Count())
        {
            ChangeSubLevelState(SubLevelState.Answering);
            currentIndexPointToHit = 0;
        }
        else
        {
            currentIndexPointToHit = 0;

            for (int i = 0; i < currentPointToHits.Count(); i++)
            {
                currentPointToHits[i].GetComponent<Image>().color = blueColor;
                currentPointToHits[i].gameObject.SetActive(true);
            }

            SetShapeAlpha(0f);
            currentPointToHits[0].GetComponent<Image>().color = Color.cyan;
        }
    }

    public void OnAnswerClick(int answerIndex)
    {
        if (IsAnswerCorrect(answerIndex))
        {
            ChangeState(LevelState.Passed);
        }
        else
            ChangeState(LevelState.Fail);
    }

    public void OnReplayClick()
    {
        // Reset all sprites and question
        caca.Close();
        enemy.Close();
        HideSprite(currentQuestionShape);
        HideSprite(questionPanel);
        for (int i = 0; i < answerPanel.transform.childCount; i++)
            answerPanel.transform.GetChild(i).GetComponent<IAnimate>().Close();
        HideSprite(answerPanel);

        // Reset all state
        currentQuestionIndex = 0;
        currentIndexPointToHit = 0;
        currentTime = 0;
        mistake = 0;
        isTimerActive = false;

        levelData.isSolved = starIsSolvedState;
        levelData.isRightInTime = starIsRightInTimeState;
        levelData.isSolved = starIsNoMistakeState;

        ChangeState(LevelState.Initialization);
    }
    #endregion

    #region Utilities

    private void PrepareQuestionAndSprite()
    {
        // Init the current variables
        currentQuestion = questions[currentQuestionIndex].question;
        currentAnswerType = questions[currentQuestionIndex].answerType;
        currentAnswers = questions[currentQuestionIndex].answers;
        currentValidAnswerIndex = questions[currentQuestionIndex].validAnswerIndex;
        currentQuestionShape = questions[currentQuestionIndex].shape;
        currentPointToHits = questions[currentQuestionIndex].pointToHits;
    }

    private void GenerateSprite()
    {
        // Load the shape and the points
        ShowSprite(currentQuestionShape);

        // Show all points
        for (int i = 0; i < currentQuestionShape.transform.childCount; i++)
        {
            currentQuestionShape.transform.GetChild(i).gameObject.SetActive(true);
        }

        SetShapeAlpha(0f);

        // Set active point which to be hit;
        currentPointToHits[0].GetComponent<Image>().color = Color.cyan;

    }

    private void GenerateQuestion()
    {
        // Set Question and Answer + Show Panels
        questionPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{currentQuestion}";
        ShowSprite(questionPanel);

        // Load the panel and the answers
        answerPanel.Load();

        for (int i = 0; i < answerPanel.transform.childCount; i++)
        {
            var child = answerPanel.transform.GetChild(i);

            switch (currentAnswerType)
            {
                // child.GetChild(0).gameObject -> Shape Placeholder
                // child.GetChild(1).gameObject -> Text Placeholder
                case AnswerType.Image:
                    child.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Levels/Level11/{currentAnswers[i]}");
                    child.GetChild(0).gameObject.SetActive(true);
                    child.GetChild(1).gameObject.SetActive(false);
                    break;
                case AnswerType.Text:
                    child.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentAnswers[i];
                    child.GetChild(1).gameObject.SetActive(true);
                    child.GetChild(0).gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            child.GetComponent<IAnimate>().Load();
        }
    }

    private async void ResetAllQuestionAndSprite()
    {
        HideSprite(currentQuestionShape);
        HideSprite(questionPanel);
        for (int i = 0; i < answerPanel.transform.childCount; i++)
            answerPanel.transform.GetChild(i).GetComponent<IAnimate>().Close();

        await Task.Delay(500);
        HideSprite(answerPanel);
    }

    private void SetShapeAlpha(float amount)
    {
        Color currentColor = currentQuestionShape.GetComponent<Image>().color;
        currentQuestionShape.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, (float)currentIndexPointToHit / (float)(currentPointToHits.Count() - 1));
    }

    protected bool IsAnswerCorrect(int answerIndex)
    {
        return answerIndex == currentValidAnswerIndex;
    }

    #endregion
}



