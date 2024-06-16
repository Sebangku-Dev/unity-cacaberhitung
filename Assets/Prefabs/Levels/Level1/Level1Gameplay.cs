using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Level1Gameplay : BaseGameplay
{
    [SerializeField] GetInferenceModel model;
    [SerializeField] DrawAstexture texture;
    [SerializeField] GameObject CandleContainer, Candle;
    [SerializeField] int minCandles, maxCandles, state, maxState;
    [SerializeField] MeshRenderer DrawCanvas;

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

        GenerateCandles();

        DrawCanvas.enabled = true;
        texture.ResetTexture();

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
            ChangeState(LevelState.Prepare);
            state++;
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

        DrawCanvas.enabled = false;

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

    public void OnClickConfirm()
    {
        int answer = CandleContainer.transform.childCount;

        ChangeState(model.prediction.predictedValue == answer ? LevelState.Passed : LevelState.Fail);
    }

    #endregion

    #region Utilities

    private void GenerateCandles()
    {
        foreach (Transform child in CandleContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i <= UnityEngine.Random.Range(minCandles, maxCandles); i++)
        {
            GameObject candle = Instantiate(Candle, CandleContainer.transform);
            candle.GetComponent<PopupAnimation>().Load();
        }
    }

    #endregion
}
