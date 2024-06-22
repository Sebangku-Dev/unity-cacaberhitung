using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class Level3Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public Sprite cakeSprite;
    }

    [System.Serializable]
    public enum CakeType
    {
        Small, Big
    }

    [Header("Level3")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private Plate smallPlate, bigPlate;
    [SerializeField] private Cake currentBigCake, currentSmallCake, temporaryCake;
    [SerializeField] private TextMeshProUGUI hintText;


    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] Answer</para>
    /// <para>[2] Hint Text</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 0;
    private float currentBigCakeFillAmount = 0f;
    private float currentSmallCakeFillAmount = 0f;

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        OnAfterLevelStateChanged += OnAfterStateChanged;

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
        OnAfterLevelStateChanged -= OnAfterStateChanged;

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

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        bigPlate.Load();
        smallPlate.Load();

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

        OnPassed?.Invoke();

        // Wait for passed animation
        await Task.Delay(1000);

        bigPlate.Close();
        smallPlate.Close();

        // Wait for plate animation
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

        base.HandleEnded();
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

        SetBigCake();
        SetSmallCake();

        // Set question
        hintText.text = $"Caca ingin {currentQuestion[2]} kue";
    }

    public void AddCakeFraction(Cake cake)
    {
        if (cake == currentBigCake)
        {
            currentBigCakeFillAmount += float.Parse(currentQuestion[0]);
        }
        else if (cake == currentSmallCake)
        {
            currentSmallCakeFillAmount += float.Parse(currentQuestion[0]);

        }
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
            currentSmallCake.gameObject.GetComponent<Draggable>().isLocked = false;
        }
        else
        {
            // Change bottom cake as answer
            currentSmallCakeFillAmount = 0f;
            currentSmallCake.gameObject.GetComponent<Image>().fillAmount = currentSmallCakeFillAmount;
            currentSmallCake.gameObject.GetComponent<Draggable>().isLocked = true;
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
            currentBigCake.gameObject.GetComponent<Draggable>().isLocked = true;
        }
        else
        {
            // Change top cake as question
            currentBigCakeFillAmount = 1f;
            currentBigCake.gameObject.GetComponent<Image>().fillAmount = currentBigCakeFillAmount;
            currentBigCake.gameObject.GetComponent<Draggable>().isLocked = false;
        }
    }


    /// <summary>
    /// If true, change to passed level state
    /// </summary>
    private void CheckAnswer()
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

    public void OnCakeBeginDrag()
    {
        // For 4+ currentQuestionIndex
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
        // Set cake fraction based on saved fill amount state
        currentSmallCake.gameObject.GetComponent<Image>().fillAmount = currentSmallCakeFillAmount;
        currentBigCake.gameObject.GetComponent<Image>().fillAmount = currentBigCakeFillAmount;

        temporaryCake.gameObject.SetActive(false);

        CheckAnswer();
    }

    #endregion

}
