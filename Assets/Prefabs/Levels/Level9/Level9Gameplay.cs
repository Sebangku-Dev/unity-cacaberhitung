using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum AritmathicType { Additional, Substraction, Times }
public class Level9Gameplay : BaseGameplay
{
    [SerializeField] TMP_InputField IFFirstNumber, IFSecondNumber, IFResultNumber;
    [SerializeField] TextMeshProUGUI TextDialog, TextOperation;
    [SerializeField] int maxState;
    AritmathicType questionType;
    int firstNumber, secondNumber, resultNumber;
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

        GenerateQuestion();

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

    public void OnCheckAnswer()
    {
        ChangeState(IFFirstNumber.text == firstNumber.ToString() && IFSecondNumber.text == secondNumber.ToString() && IFResultNumber.text == resultNumber.ToString() ? LevelState.Passed : LevelState.Fail);
    }

    public void OnClearAnswer()
    {
        IFResultNumber.Select();
        IFResultNumber.text  = "";

        IFFirstNumber.Select();
        IFFirstNumber.text  = "";

        IFSecondNumber.Select();
        IFSecondNumber.text  = "";
    }

    #endregion

    #region Utilities

    void GenerateQuestion()
    {
        Array values = Enum.GetValues(typeof(AritmathicType));
        questionType = (AritmathicType)values.GetValue(UnityEngine.Random.Range(0, 3));

        switch (questionType)
        {
            case AritmathicType.Additional:
                AdditionalQuestion();
                break;
            case AritmathicType.Substraction:
                SubstractionQuestion();
                break;
            case AritmathicType.Times:
                TimesQuestion();
                break;
            default:
                break;
        }
    }

    void AdditionalQuestion()
    {
        resultNumber = UnityEngine.Random.Range(1, 21);
        firstNumber = UnityEngine.Random.Range(1, 20);
        secondNumber = resultNumber - firstNumber;

        TextDialog.text = "kamu harus berjalan sebanyak " + firstNumber + " langkah. Kemudian " + secondNumber + " langkah lagi!";
        TextOperation.text = "+";
    }
    void SubstractionQuestion()
    {
        firstNumber = UnityEngine.Random.Range(1, 21);
        secondNumber = UnityEngine.Random.Range(1, firstNumber);
        resultNumber = firstNumber - secondNumber;

        TextDialog.text = "kamu harus berjalan sebanyak " + firstNumber + " langkah. Kemudian kembali sebanyak " + secondNumber + " langkah!";
        TextOperation.text = "-";
    }
    void TimesQuestion()
    {
        firstNumber = UnityEngine.Random.Range(1, 6);
        secondNumber = UnityEngine.Random.Range(1, 5);
        resultNumber = firstNumber * secondNumber;

        TextDialog.text = "kamu harus berjalan " + firstNumber + " langkah. Sebanyak " + secondNumber + " kali berulang!";
        TextOperation.text = "x";
    }

    #endregion
}
