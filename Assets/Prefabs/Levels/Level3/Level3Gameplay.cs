using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;

public class Level3Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public LevelSprite questionSprite;
    }

    [Header("Level3")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] private Cake bigCakePrefab;
    [SerializeField] private Cake smallCakePrefab;
    [SerializeField] private Plate platePrefab;
    [SerializeField] private Plate smallPlatePrefab;


    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] Answer</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex;
    private LevelSprite currentQuestionSprite;
    private Cake currentBigCake;



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

    private void OnBeforeStateChanged(LevelState changedState)
    {

    }
    #endregion

    #region Utilities
    private void GenerateQuestion()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSprite = questions[currentQuestionIndex].questionSprite;

        AddBigCake(platePrefab);
        AddSmallCake(smallPlatePrefab);

        // Reset current plate's cake fill amount to 0
        currentBigCake = platePrefab.GetComponentInChildren<Cake>();
        currentBigCake.gameObject.GetComponent<Image>().fillAmount = 0f;
    }

    public void AddFraction()
    {
        currentBigCake.gameObject.GetComponent<Image>().fillAmount += float.Parse(currentQuestion[0]);
    }


    public void AddSmallCake(Plate plate)
    {
        Instantiate(smallCakePrefab, plate.transform);
    }

    public void AddBigCake(Plate plate)
    {
        Instantiate(bigCakePrefab, plate.transform);
    }

    #endregion

}
