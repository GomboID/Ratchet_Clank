using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private GameObject[] m_Levels;

    private int m_CurrentLevel;
    private LevelScript m_LevelScript;
    public LevelScript GetLevel => m_LevelScript;

    protected override void Awake()
    {
        base.Awake();
        CreateLevel();
    }

    private void CreateLevel()
    {
        m_CurrentLevel = PlayerPrefs.GetInt(Constants.CurrentLevel, 0);
        int convertedIndex = m_CurrentLevel - (int)(m_CurrentLevel / m_Levels.Length) * m_Levels.Length;
        m_LevelScript = Instantiate(m_Levels[convertedIndex], transform).GetComponent<LevelScript>();
    }
}
