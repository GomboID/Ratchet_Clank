using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private int m_CurrentLevel = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    

    public void LevelStart(int _currentLevel)
    {
        m_CurrentLevel = _currentLevel;
    }

    public void LevelComplete(int _starCount)
    {

    }

    public void LevelFail()
    {

    }

    public void RewardOffer(string _placement)
    {

    }

    public void TutorialStep(string _tutorialName, string _stepName)
    {

    }

    public void TutorialComplete()
    {

    }

    public void AppLoaded()
    {

    }
}
