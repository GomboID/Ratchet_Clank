using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataController : Singleton<DataController>
{
    [SerializeField] private Material m_HologramMat, m_HeavinessMat, m_DefaultMat, m_DeadMat;
    [SerializeField] private DragItemIcon[] m_DragIcons;
    [SerializeField] private MapColorScheme[] m_Schemes;
    [SerializeField] private int[] m_StarsToOpenLocation;

    private int m_CurrentStars;
    private MapColorScheme m_CurrentScheme;

    public MapColorScheme GetCurrentCheme => m_CurrentScheme;

    public int GetStarsCount => m_CurrentStars;

    protected override void Awake()
    {
        base.Awake();

        int currentLocation = PlayerPrefs.GetInt(Constants.LocationCount, 0);
        int convertedIndex = currentLocation - (int)(currentLocation / m_Schemes.Length) * m_Schemes.Length;
        m_CurrentScheme = m_Schemes[convertedIndex];
        m_CurrentStars = m_StarsToOpenLocation[convertedIndex];
    }

    public GameObject GetDragIcon(ObstacleType _type)
    {
        return m_DragIcons.FirstOrDefault(a => a.ObsType == _type).IconPrefab;
    }

    public Material GetMat(ObstacleType _type)
    {
        switch (_type)
        {
            case ObstacleType.Nothing:
                return m_DefaultMat;
            case ObstacleType.Heaviness:
                return m_HeavinessMat;
            case ObstacleType.Hologram:
                return m_HologramMat;
            case ObstacleType.NumberOfTypes:
                return m_DeadMat;
            default:
                return null;
        }
    }
}

[Serializable]
public struct DragItemIcon
{
    public ObstacleType ObsType;
    public GameObject IconPrefab;
}

[Serializable]
public struct MapColorScheme
{
    public Material MainMat;
    public Material ActiveButtons;
}
