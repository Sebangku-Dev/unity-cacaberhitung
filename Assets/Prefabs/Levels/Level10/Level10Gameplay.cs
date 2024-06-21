using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;

public class Level10Gameplay : BaseGameplay
{
    [System.Serializable]
    public class Questions
    {
        public string questionString;
        public LevelSprite questionSprite;
    }

    [Header("Level 10")]
    [SerializeField] private List<Questions> questions;
    [SerializeField] CharacterHintAnimation caca;
    [SerializeField] CharacterHintAnimation otherCharacter;
    [SerializeField] PanAnimation background;
    [SerializeField] LevelSprite draggableSlots;
    [SerializeField] LevelSprite hint;

    /// <summary>
    /// Format:
    /// <para>[0] Question</para> 
    /// <para>[1] <see cref="ArithmethicBlock"/> (0)</para>
    /// <para>[2] <see cref="ArithmethicBlock"/> (1)</para>
    /// <para>[3] <see cref="ArithmethicBlock"/> (2)</para>
    /// <para>[4] <see cref="ArithmethicBlock"/> (3)</para>
    /// <para>[5] <see cref="ArithmethicBlock"/> (4)</para>
    /// </summary>
    private List<string> currentQuestion;
    private int currentQuestionIndex = 0;
    private LevelSprite currentQuestionSpriteRef;
    private LevelSprite currentQuestionSprite;
    private List<ArithmethicBlock> arithmethicBlocks = new List<ArithmethicBlock>();

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        OnBeforeLevelStateChanged += OnBeforeStateChanged;
        OnAfterLevelStateChanged += OnAfterStateChanged;
        Draggable.OnItemEndDrag += ArithmethicBlockChecker;
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
        Draggable.OnItemEndDrag -= ArithmethicBlockChecker;
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

        // Generate everything: question and hint sprite, draggable, slot, etc
        GenerateQuestionAndSprites();

        // Invoke OnPrepare
        OnPrepare?.Invoke();

        // In-game cutscene
        if (currentQuestionIndex == 0)
        {
            caca.Load();
            otherCharacter.Load();
            await LockNumberBlocks(8000);
            background.Pan(420);
            await Task.Delay(1000);

        }
        else
        {
            await LockNumberBlocks(500);
        }

        // Show question sprites and hint
        ShowSprite(hint);
        ShowSprite(currentQuestionSprite);
        ShowSprite(draggableSlots);

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
        HideSprite(draggableSlots);

        // Hide all number blocks
        foreach (var arithmethicBlock in arithmethicBlocks)
        {
            arithmethicBlock.GetComponent<IAnimate>().Close();
        }

        // Wait for passed animation
        await LockNumberBlocks(2000);

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

    #region User Interaction

    private bool[] isAnswerTrues = new bool[5];
    private bool isFulfilled = false;

    /// <summary>
    /// Used in each <see cref="DraggableSlot"/>
    /// </summary>
    public async void ArithmethicBlockChecker()
    {
        for (int i = 0; i < draggableSlots.transform.childCount; i++)
        {
            isAnswerTrues[i] = draggableSlots.transform.GetChild(i).GetComponent<DraggableSlot>()?.GetComponentInChildren<Draggable>()?.GetComponentInChildren<TextMeshProUGUI>()?.text == currentQuestion[i + 1];
        }

        await Task.Delay(100);

        // If draggables parent has no child then all must be fulfilled
        isFulfilled = currentQuestionSprite.transform.childCount == 0;

        if (isFulfilled)
        {
            if (!isAnswerTrues.Contains(false))
            {
                ChangeState(LevelState.Passed);
            }
            else
            {
                ChangeState(LevelState.Fail);
            }
        }

    }
    #endregion


    #region Utilities
    private void GenerateQuestionAndSprites()
    {
        currentQuestion = questions[currentQuestionIndex].questionString.Split(";").ToList();
        currentQuestionSpriteRef = questions[currentQuestionIndex].questionSprite;

        currentQuestionSprite = GenerateSprite(currentQuestionSpriteRef, currentQuestionSpriteRef.transform.parent, currentQuestionSpriteRef.transform.position);

        // Set question text
        // (Not recommended due to too many specified nested children)
        hint
        .GetComponent<CharacterHintAnimation>()
        .child
        .GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[0];

        // Draggable slots and draggables must have the same count
        for (int i = 0; i < currentQuestionSprite.transform.childCount; i++)
        {
            currentQuestionSprite.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion[i + 1];
            arithmethicBlocks.Add(currentQuestionSprite.transform.GetChild(i).GetComponent<ArithmethicBlock>());
            // draggableSlots.transform.GetChild(i).GetComponent<DraggableSlot>().relatedDraggable = arithmethicBlocks[i].GetComponent<Draggable>();
        }
    }


    private void DestroyAllNumberBlocks()
    {
        arithmethicBlocks.RemoveAll((ArithmethicBlock _) => true);
    }

    private async Task LockNumberBlocks(int delayMs)
    {
        foreach (var arithmethicBlock in arithmethicBlocks)
        {
            arithmethicBlock.SetAlpha(0.5f);
            arithmethicBlock.GetComponent<Draggable>().isLocked = true;
        }

        await Task.Delay(delayMs);

        foreach (var arithmethicBlock in arithmethicBlocks)
        {
            arithmethicBlock.SetAlpha(1f);
            arithmethicBlock.GetComponent<Draggable>().isLocked = false;
        }
    }


    #endregion

}
