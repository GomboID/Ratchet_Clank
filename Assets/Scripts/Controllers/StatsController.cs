using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : Singleton<StatsController>
{
    private int m_ComoleteCount = 1;
    private bool m_IsAppLoaded = false;

    public int GetCompleteLevels => m_ComoleteCount;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void SendLoadedEvent()
    {
        if (!m_IsAppLoaded)
        {
            m_IsAppLoaded = true;
            EventManager.Instance.AppLoaded();
        }
    }

    public void LevelComplete()
    {
        m_ComoleteCount++;
    }

    public void ResetLevel()
    {
        m_ComoleteCount = 1;
    }
}
