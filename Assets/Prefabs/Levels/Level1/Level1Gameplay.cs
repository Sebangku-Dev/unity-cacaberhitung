using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Level1Gameplay : BaseGameplay
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI stateText;

    #region MonoBehaviour 
    protected override void Awake()
    {
        base.Awake();
        BaseGameplay.OnStateChanged += OnLevelStateChanged;
    }

    private void Start()
    {
        BaseGameplay.Instance.ChangeState(LevelState.Prepare);
    }

    private void Update()
    {
        stateText.text = BaseGameplay.Instance.CurrentLevelState.ToString();
    }

    private void OnDestroy()
    {
        BaseGameplay.OnStateChanged -= OnLevelStateChanged;
    }
    #endregion


    #region Level State
    protected override void HandlePrepare()
    {
        base.HandlePrepare();

        questionText.text = "8+2=?";

        BaseGameplay.Instance.ChangeState(LevelState.UserInteraction);
    }

    protected override void HandleUserInteraction()
    {
        base.HandleUserInteraction();
    }

    protected override void HandlePassed()
    {
        base.HandlePassed();

        Debug.Log("Pintar");
    }
    protected override void HandleFail()
    {
        base.HandleFail();

        Debug.Log("Belajar lagi ya");
    }

    private void OnLevelStateChanged(LevelState changedState)
    {

    }
    #endregion

    #region User Interaction to Canvas
    public void OnAnswerClick(TextMeshProUGUI buttonText)
    {
        if (buttonText.text == "Benar")
        {
            BaseGameplay.Instance.ChangeState(LevelState.Passed);
        }
        else
        {
            BaseGameplay.Instance.ChangeState(LevelState.Fail);
        }
    }
    #endregion
}
