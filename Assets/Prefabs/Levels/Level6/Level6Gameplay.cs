using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public enum PlaneStepType { Compare, Dot, BasicSeq, OddSeq, EvenSeq, AddSeq, Fibonacci }

public class Level6Gameplay : BaseGameplay
{
    [SerializeField] GameObject[] Planes;
    [SerializeField] GameObject PlaneContainer, DragablePlaneContainer, BasicPlane, Dot;
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
        ResetPlane();
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

    public void OnCheckPlane()
    {
        for (int i = 0; i < PlaneContainer.transform.childCount; i++)
        {
            if (!Mathf.Approximately(PlaneContainer.transform.GetChild(i).gameObject.GetComponent<CanvasGroup>().alpha, 1.0f))
            {
                return;
            }
        }

        ChangeState(LevelState.Passed);
    }

    #endregion

    #region Utilities

    void ResetPlane()
    {
        if (PlaneContainer.transform.childCount > 0)
        {
            for (int i = 0; i < PlaneContainer.transform.childCount; i++)
            {
                Destroy(PlaneContainer.transform.GetChild(i).gameObject);
            }
        }
    }

    void HandleStepState(PlaneStepType step)
    {
        switch (step)
        {
            case PlaneStepType.Compare:
                GeneratePlaneToCompare();
                break;
            default:
                GenerateBasicPlane(step);
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
        BasePlane.GetComponent<DraggableSlot>().isDisabled = false;
        BasePlane.GetComponent<DraggableSlot>().relatedDraggable = Dragable.GetComponent<Draggable>();
        BasePlane.GetComponent<DraggableSlot>().OnDropItem.AddListener(() => OnDraggableDropped(BasePlane, Dragable));
    }

    public void OnDraggableDropped(GameObject basePlane, GameObject draggable)
    {
        basePlane.GetComponent<CanvasGroup>().alpha = 1.0f;
        Destroy(draggable);

        OnCheckPlane();
    }

    void GeneratePlaneToCompare()
    {
        int blankCount = 0;
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

            if (blankCount == 0 && i == (IsToLargest ? Planes.Length - 1 : 0)) SetAsDragable(plane);

            i += (IsToLargest ? 1 : -1);
        }
    }

    void GenerateBasicPlane(PlaneStepType step)
    {
        int blankCount = 0, maxBlank = UnityEngine.Random.Range(1, 3);

        int initNumber = UnityEngine.Random.Range(IsToLargest ? 1 : 5, IsToLargest ? 17 : 21);

        for (int i = 0; i < 5; i++)
        {
            GameObject plane = Instantiate(BasicPlane, PlaneContainer.transform);

            switch (step)
            {
                case PlaneStepType.Dot:
                    GenerateDot(i, plane);
                    break;
                default:
                    GenerateNumber(step, i, initNumber, plane);
                    break;
            }

            bool isBlankPlane = UnityEngine.Random.Range(0, 2) == 0;

            if (isBlankPlane && blankCount < maxBlank)
            {
                SetAsDragable(plane);
                blankCount++;
            }
        }
    }

    void GenerateDot(int index, GameObject plane)
    {
        for (int i = IsToLargest ? 0 : 5; IsToLargest ? i <= index : i > index; i += IsToLargest ? 1 : -1)
        {
            Instantiate(Dot, plane.transform.GetChild(1));
        }
    }

    void GenerateNumber(PlaneStepType step, int index, int initNumber, GameObject plane)
    {
        switch (step)
        {
            case PlaneStepType.BasicSeq:
                plane.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = (initNumber + index).ToString();
                break;
            case PlaneStepType.OddSeq:
                plane.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ((initNumber % 2 == 1 ? initNumber : initNumber + 1) + index * 2).ToString();
                break;
            case PlaneStepType.EvenSeq:
                plane.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ((initNumber % 2 == 0 ? initNumber : initNumber + 1) + index * 2).ToString();
                break;
            case PlaneStepType.AddSeq:
                plane.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = (initNumber + (index * 2)).ToString();
                break;
            case PlaneStepType.Fibonacci:
                int fib = FibonacciSeries(index, initNumber);
                plane.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = fib.ToString();
                break;
            default:
                break;
        }
    }

    int FibonacciSeries(int count, int initNumber)
    {
        int fib = 0, FirstNumber = 0, SecondNumber = initNumber;
        if (count == 0) return FirstNumber;
        else if(count==1) return SecondNumber;
        else
        {
             for(int i = 2; i < count; i++)
                {
                    fib = FirstNumber + SecondNumber;
                    FirstNumber = SecondNumber;
                    SecondNumber = fib;
                }
        }
        return fib;
    }

    #endregion
}
