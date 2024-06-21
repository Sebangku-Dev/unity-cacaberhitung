using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Level7Gameplay : BaseGameplay
{
    [SerializeField] TextMeshProUGUI[] TextWeights;
    [SerializeField] TextMeshProUGUI TextAnswer;
    int[] Weights = new int[3];
    int WeightsSum;
    [SerializeField] int maxState;
    [SerializeField] protected UnityEvent WhenChangeAnswer;
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

        GenerateWeight();

        OnPrepare?.Invoke();

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

        if (state < maxState)
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
    #endregion

    private async Task DelayAnswer(float interStateDelay)
    {
        //state init

        await Task.Delay(Mathf.RoundToInt(interStateDelay));

        //state after delay
    }

    #region User Interaction

    public void OnClickAnswer()
    {
        ChangeState(TextAnswer.text == WeightsSum.ToString() ? LevelState.Passed : LevelState.Fail);
    }

    public void OnChangeAnswer(int count)
    {
        int key = Int32.Parse(TextAnswer.text) + count;
        TextAnswer.text = key.ToString();

        WhenChangeAnswer?.Invoke();
    }

    #endregion

    #region Utilities

    void GenerateWeight()
    {
        WeightsSum = 0;

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = UnityEngine.Random.Range(1, 5);
            WeightsSum += Weights[i];
        }

        Array.Sort(Weights);

        int index = 0;
        foreach (TextMeshProUGUI weight in TextWeights)
        {
            weight.text = Weights[index].ToString();
            index++;
        }
    }

    #endregion
}
