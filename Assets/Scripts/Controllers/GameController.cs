using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : Singleton<GameController>
{
    public event Action Action_GameStart, Action_Win, Action_Lose, Action_GameEnd;

    private GameState m_CurrentState = GameState.Default;

    public GameState GetState => m_CurrentState;

    private void Start()
    {
        Application.targetFrameRate = 60;
        StatsController.Instance.SendLoadedEvent();
        ADSManager.Instance.ShowInterstitial();
    }

    public void GameStart()
    {
        Action_GameStart?.Invoke();
        m_CurrentState = GameState.Game;
        EventManager.Instance.LevelStart(PlayerPrefs.GetInt(Constants.CurrentLevel, 0) + 1);
    }

    public void Win()
    {
        if (m_CurrentState != GameState.Game)
            return;
        StatsController.Instance.LevelComplete();
        Action_GameEnd?.Invoke();
        Action_Win?.Invoke();
        m_CurrentState = GameState.EndGame;
        EventManager.Instance.LevelComplete(Mathf.Clamp(GamePlayUI.Instance.GetAttempts, 0, 3));        
    }

    public void Lose()
    {
        if (m_CurrentState != GameState.Game)
            return;

        Action_GameEnd?.Invoke();
        Action_Lose?.Invoke();
        m_CurrentState = GameState.EndGame;
        EventManager.Instance.LevelFail();
    }
}
