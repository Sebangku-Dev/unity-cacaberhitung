using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class Level5Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public LevelSprite questionSprite;
    }

    [Header("Level3")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] LevelSprite hint;
    [SerializeField] DraggableSlot hundredDraggableSlot, tenDraggabeSlot, oneDraggableSlot;
    [SerializeField] CacaHintAnimation caca;

    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] Hundred</para>
    /// <para>[2] Ten</para>
    /// <para>[2] One</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 4;
    private LevelSprite currentQuestionSpriteRef;
    private LevelSprite currentQuestionSprite;
    private List<NumberBlock> numberBlocks = new List<NumberBlock>();

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        OnAfterLevelStateChanged += OnAfterStateChanged;
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

        GenerateQuestionAndSprites();
        ShowSprite(hint);

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        if (currentQuestionIndex == 0)
        {
            caca.Load();
            await LockNumberBlocks(8000);
        }
        else
        {
            await LockNumberBlocks(2000);
        }

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
        HideSprite(hint);

        // Hide all number blocks
        foreach (var numberBlock in numberBlocks)
        {
            numberBlock.GetComponent<IAnimate>().Close();
        }

        // Wait for passed animation

        await LockNumberBlocks(3000);

        DestroySprite(currentQuestionSprite); // Destroy the number blocks envelope
        DestroyAllNumberBlocks(); // Destroy the actual number blocks

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

    private void OnAfterStateChanged(LevelState changedState)
    {

    }
    #endregion

    #region Utilities
    private void GenerateQuestionAndSprites()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSpriteRef = questions[currentQuestionIndex].questionSprite;

        currentQuestionSprite = GenerateSprite(currentQuestionSpriteRef, currentQuestionSpriteRef.transform.parent, currentQuestionSpriteRef.transform.position);

        // Set question text
        // Set specific draggable slot to each number block
        hint.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[0];
        foreach (var numberBlock in currentQuestionSprite.GetComponentsInChildren<NumberBlock>())
        {
            if (numberBlock.numberBlockType == NumberBlock.Type.Hundred)
            {
                numberBlock.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[1];
                hundredDraggableSlot.relatedDraggable = numberBlock.GetComponent<Draggable>();
            }

            if (numberBlock.numberBlockType == NumberBlock.Type.Ten)
            {
                numberBlock.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[2];
                tenDraggabeSlot.relatedDraggable = numberBlock.GetComponent<Draggable>();
            }

            if (numberBlock.numberBlockType == NumberBlock.Type.One)
            {
                numberBlock.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[3];
                oneDraggableSlot.relatedDraggable = numberBlock.GetComponent<Draggable>();
            }

            numberBlocks.Add(numberBlock);
        }

        ShowSprite(currentQuestionSprite);
    }

    private void DestroyAllNumberBlocks()
    {
        if (hundredDraggableSlot.transform.childCount > 0) Destroy(hundredDraggableSlot.transform.GetChild(0).gameObject);
        if (tenDraggabeSlot.transform.childCount > 0) Destroy(tenDraggabeSlot.transform.GetChild(0).gameObject);
        if (oneDraggableSlot.transform.childCount > 0) Destroy(oneDraggableSlot.transform.GetChild(0).gameObject);

        numberBlocks.RemoveAll((NumberBlock _) => true);
    }

    private async Task LockNumberBlocks(int delayMs)
    {
        foreach (var numberBlock in numberBlocks)
        {
            numberBlock.SetAlpha(0.5f);
            numberBlock.GetComponent<Draggable>().isLocked = true;
        }

        await Task.Delay(delayMs);

        foreach (var numberBlock in numberBlocks)
        {
            numberBlock.SetAlpha(1f);
            numberBlock.GetComponent<Draggable>().isLocked = false;
        }
    }


    /// <summary>
    /// Used by <see cref="MultipleDraggableSlot"/>
    /// </summary>
    public async void NumberBlockChecker()
    {
        if (currentQuestionSprite.transform.childCount < 1)
        {
            // Wait draggable object until it snapped
            await Task.Delay(100);

            ChangeState(LevelState.Passed);
        }
    }

    #endregion

}
