using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Level3Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public LevelSprite questionSprite;
        public Sprite cakeSprite;
    }

    [System.Serializable]
    public enum CakeType
    {
        Small, Big
    }

    [Header("Level3")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private Cake currentBigCake;
    [SerializeField] private Cake currentSmallCake;
    [SerializeField] private Cake temporaryCake;
    [SerializeField] private Plate plateGameobject;
    [SerializeField] private Plate smallPlateGameobject;


    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] Answer</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 0;
    private LevelSprite currentQuestionSprite;
    private float currentBigCakeFillAmount = 0f;
    private float currentSmallCakeFillAmount = 0f;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        Draggable.OnItemBeginDrag += OnCakeBeginDrag;
        Draggable.OnItemEndDrag += OnCakeEndDrag;
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
        Draggable.OnItemBeginDrag -= OnCakeBeginDrag;
        Draggable.OnItemEndDrag -= OnCakeEndDrag;
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

        await Task.Delay(2000);

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
        int duration = 2000;
        await Task.WhenAll(
            new Task[] { Task.Delay(duration), LockDraggable() }
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
    #endregion

    #region Utilities
    private void GenerateQuestion()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSprite = questions[currentQuestionIndex].questionSprite;

        SetBigCake();
        SetSmallCake();
    }

    public void AddCakeFraction(Cake cake)
    {
        if (cake == currentBigCake)
            currentBigCakeFillAmount += float.Parse(currentQuestion[0]);
        else if (cake == currentSmallCake)
            currentSmallCakeFillAmount += float.Parse(currentQuestion[0]);
    }

    public void SubstractCakeFraction(Cake cake)
    {
        if (cake == currentBigCake)
            currentBigCakeFillAmount -= float.Parse(currentQuestion[0]);
        else if (cake == currentSmallCake)
            currentSmallCakeFillAmount -= float.Parse(currentQuestion[0]);
    }

    public void SetSmallCake()
    {
        currentSmallCake.gameObject.GetComponent<Image>().sprite = questions[currentQuestionIndex].cakeSprite;
        currentSmallCake.GetComponent<Draggable>().isNotSnapped = true;

        if (currentQuestionIndex < 4)
        {
            currentSmallCakeFillAmount = float.Parse(currentQuestion[0]);
            currentSmallCake.gameObject.GetComponent<Image>().fillAmount = currentSmallCakeFillAmount;
            currentSmallCake.GetComponent<Draggable>().isLocked = false;
        }
        else
        {
            // Change bottom cake as answer
            currentSmallCakeFillAmount = 0f;
            currentSmallCake.gameObject.GetComponent<Image>().fillAmount = currentSmallCakeFillAmount;
            currentSmallCake.GetComponent<Draggable>().isLocked = true;
        }
    }

    public void SetBigCake()
    {
        currentBigCake.gameObject.GetComponent<Image>().sprite = questions[currentQuestionIndex].cakeSprite;
        temporaryCake.gameObject.GetComponent<Image>().sprite = questions[currentQuestionIndex].cakeSprite;
        currentBigCake.GetComponent<Draggable>().isNotSnapped = true;

        if (currentQuestionIndex < 4)
        {
            currentBigCakeFillAmount = 0f;
            currentBigCake.gameObject.GetComponent<Image>().fillAmount = currentBigCakeFillAmount;
            currentBigCake.GetComponent<Draggable>().isLocked = true;
        }
        else
        {
            // Change top cake as question
            currentBigCakeFillAmount = 1f;
            currentBigCake.gameObject.GetComponent<Image>().fillAmount = currentBigCakeFillAmount;
            currentBigCake.GetComponent<Draggable>().isLocked = false;
        }
    }


    /// <summary>
    /// If true, change to passed level state
    /// </summary>
    public void CheckAnswer()
    {

        if (currentQuestionIndex < 4 && Mathf.Approximately(currentBigCakeFillAmount, float.Parse(currentQuestion[1])))
        {
            ChangeState(LevelState.Passed);
        }
        else if (Mathf.Approximately(currentBigCakeFillAmount, float.Parse(currentQuestion[1])))
        {
            ChangeState(LevelState.Passed);
        }
        else
        {
            ChangeState(LevelState.UserInteraction);
        }
    }

    private async Task LockDraggable()
    {
        currentSmallCake.GetComponent<Draggable>().isLocked = true;
        currentBigCake.GetComponent<Draggable>().isLocked = true;

        await Task.Delay(2000);

        currentSmallCake.GetComponent<Draggable>().isLocked = false;
        currentBigCake.GetComponent<Draggable>().isLocked = false;
    }

    public void OnCakeBeginDrag()
    {
        if (!(currentQuestionIndex < 4) && CurrentLevelState != LevelState.Passed)
        {
            // Prepare and save latest state
            temporaryCake.gameObject.SetActive(true);
            currentBigCakeFillAmount = currentBigCake.GetComponent<Image>().fillAmount;
            currentSmallCakeFillAmount = currentSmallCake.GetComponent<Image>().fillAmount;

            // Temporary change fill amount when on drag
            temporaryCake.GetComponent<Image>().fillAmount = currentBigCakeFillAmount - float.Parse(currentQuestion[0]);
            currentBigCake.GetComponent<Image>().fillAmount = float.Parse(currentQuestion[0]);
        }
    }

    public void OnCakeEndDrag()
    {
        currentSmallCake.gameObject.GetComponent<Image>().fillAmount = currentSmallCakeFillAmount;
        currentBigCake.gameObject.GetComponent<Image>().fillAmount = currentBigCakeFillAmount;

        temporaryCake.gameObject.SetActive(false);
    }

    #endregion

}
