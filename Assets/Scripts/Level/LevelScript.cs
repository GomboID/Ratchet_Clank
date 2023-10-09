using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private Vector3 m_CameraPos = new Vector3(7.8f, 18.66f, -5f);
    [SerializeField] private ObstacleType[] m_StartModifiers;
    [SerializeField] private int m_AttemptsCount = 5;

    private Transform m_FinishTransform;

    public Vector3 GetCameraPos => m_CameraPos;
    public ObstacleType[] GetStartModifiers => m_StartModifiers;
    public int GetAttemptsCount => m_AttemptsCount;

    public Vector3 GetFinishPosition => m_FinishTransform.position;

    private void Awake()
    {
        m_FinishTransform = GetComponentInChildren<FinishScript>().transform;
    }
}
