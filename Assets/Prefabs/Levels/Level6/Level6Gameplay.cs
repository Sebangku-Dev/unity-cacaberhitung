using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public enum PlaneStepType { Compare, Dot, BasicSeq, OddSeq, EvenSeq, AddSeq, Fibonacci }

public class Level6Gameplay : BaseGameplay
{
    [SerializeField] GameObject[] Planes;
    [SerializeField] GameObject PlaneContainer, DragablePlaneContainer, BasicPlane, Dot;
    [SerializeField] Canvas canvas;
    GameObject DotContainer;
    TextMeshProUGUI TextNumber;
    bool IsToLargest;

    [SerializeField] int CompareFinishAtState, DotFinishAtState, BasicSeqFinishAtState, OddSeqFinishAtState, EvenSeqFinishAtState, AddSeqFinishAtState, FibSeqFinishAtState;
    int state;

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

        GenerateIsToLargestOrSmallest();
        HandleStepState(state < CompareFinishAtState ? PlaneStepType.Compare : state < DotFinishAtState ? PlaneStepType.Dot : state < BasicSeqFinishAtState ? PlaneStepType.BasicSeq : state < OddSeqFinishAtState ? PlaneStepType.OddSeq : state < EvenSeqFinishAtState ? PlaneStepType.EvenSeq : state < AddSeqFinishAtState ? PlaneStepType.AddSeq : PlaneStepType.Fibonacci);

        await DelayAnswer(2000f);

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

        // HideSprite(hide some sprite);
        OnPassed?.Invoke();

        // Wait for a sec and delay answer to prevent user to access it
        await Task.WhenAll(new Task[] { Task.Delay(1500), DelayAnswer(2000) });

        if (state < 99)
        {
            // change gameplay index
            state++;
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

        // Add score to user current score based on true-ish boolean
        AddScore((new bool[] { levelData.isSolved, levelData.isRightInTime, levelData.isNoMistake }).Where(c => c).Count());

        // Show and unlock next level   
        ShowAndUnlockNextLevel();

        // Show ended modal
        await ShowEndedModal();
    }

    private void OnBeforeStateChanged(LevelState changedState)
    {
    }
    #endregion

    private async Task DelayAnswer(float interStateDelay)
    {
        //state init

        await Task.Delay(Mathf.RoundToInt(interStateDelay));

        //state after delay
    }

    #region User Interaction



    #endregion

    #region Utilities

    void HandleStepState(PlaneStepType step)
    {
        switch (step)
        {
            case PlaneStepType.Compare:
                GeneratePlaneToCompare();
                break;
            case PlaneStepType.Dot:
                break;
            case PlaneStepType.BasicSeq:
                break;
            case PlaneStepType.OddSeq:
                break;
            case PlaneStepType.EvenSeq:
                break;
            case PlaneStepType.AddSeq:
                break;
            case PlaneStepType.Fibonacci:
                break;
            default:
                break;
        }
    }

    void GenerateIsToLargestOrSmallest()
    {
        IsToLargest = UnityEngine.Random.Range(0, 2) == 0;
    }

    void SetAsDragable(GameObject BasePlane)
    {
        GameObject Dragable = Instantiate(BasePlane, DragablePlaneContainer.transform);
        Dragable.GetComponent<Draggable>().isLocked = false;

        BasePlane.GetComponent<CanvasGroup>().alpha = 0.0f;
        BasePlane.GetComponent<DraggableSlot>().relatedDraggable = Dragable.GetComponent<Draggable>();
    }

    void GeneratePlaneToCompare()
    {
        int index = 0, blankCount = 0;
        int i = IsToLargest ? 0 : Planes.Length - 1;
        int maxBlank = UnityEngine.Random.Range(1, 3);

        foreach (GameObject Plane in Planes)
        {
            GameObject plane = Instantiate(Planes[i], PlaneContainer.transform);

            bool isBlankPlane = UnityEngine.Random.Range(0, 2) == 0;

            if (isBlankPlane && blankCount < maxBlank)
            {
                SetAsDragable(plane);
                blankCount++;
            }

            i += (IsToLargest ? 1 : -1);
            index++;
        }
    }

    #endregion
}
