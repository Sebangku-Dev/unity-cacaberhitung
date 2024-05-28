using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class Level4Gameplay : BaseGameplay
{
    [SerializeField] GameObject[] StepToCompare;
    [SerializeField] private List<float> scales = new List<float> { 0.7f, 1.0f, 1.3f, 1.6f, 1.8f };
    bool isCompareLongest;
    [SerializeField] TextMeshProUGUI TextQuestionCompare;

    [SerializeField] GameObject StepToMeasure;
    [SerializeField] int state, CompareFinishAtState, MeasureFinishAtState;

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

        if (state < CompareFinishAtState)
        {
            ResizeStepsLength();
            SetIsLOngestOrShortest();
        }
        else if (state < MeasureFinishAtState) SetRuler();

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

        if (state < MeasureFinishAtState)
        {
            // change gameplay index
            state ++;
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

    public void OnClickCompareAnswer(int index)
    {
        GameObject correctScaleObject = StepToCompare[0];
        float correctScaleMagnitude = correctScaleObject.transform.localScale.magnitude;

        foreach (GameObject step in StepToCompare)
        {
            float currentScaleMagnitude = step.transform.localScale.magnitude;
            if (isCompareLongest ? currentScaleMagnitude > correctScaleMagnitude : currentScaleMagnitude < correctScaleMagnitude)
            {
                correctScaleObject = step;
                correctScaleMagnitude = currentScaleMagnitude;
            }
        }

        ChangeState(StepToCompare[index] == correctScaleObject ? LevelState.Passed : LevelState.Fail);
    }

    #endregion

    #region Utilities

    void ResizeStepsLength()
    {
        Shuffle(scales);

        for (int i = 0; i < StepToCompare.Length; i++)
        {
            if (i < scales.Count)
            {
                StepToCompare[i].transform.localScale = Vector3.one * scales[i];
            }
            else
            {
                Debug.LogWarning("Not enough unique scales for all GameObjects.");
                break;
            }
        }
    }

    void SetIsLOngestOrShortest()
    {
        isCompareLongest = UnityEngine.Random.Range(0, 2) == 0;

        TextQuestionCompare.text = isCompareLongest ? "Mana yang lebih panjang?" : "Mana yang lebih pendek?";
        // Debug.Log(isCompareLongest);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void SetRuler()
    {

    }

    #endregion
}
