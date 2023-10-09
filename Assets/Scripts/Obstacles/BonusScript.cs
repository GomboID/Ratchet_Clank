using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BonusScript : MonoBehaviour
{
    [SerializeField] private ObstacleType m_ObsType;
    [SerializeField] private ObstacleData[] m_Data;

    private ObstacleData m_CurrentData;

    private void Start()
    {
        m_CurrentData = m_Data.FirstOrDefault(a => a.m_ObsType == m_ObsType);
        m_CurrentData.Model.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm)
        {
            DragAndDropController.Instance.CreateNewModifier(m_ObsType);
            TutorialController.Instance.NextStep(TutorType.PickUp);
            Instantiate(m_CurrentData.PickUpEffect, transform.position + Vector3.up, Quaternion.identity, transform.parent);
            Destroy(gameObject);
        }
    }
}

[Serializable]
public class ObstacleData
{
    public ObstacleType m_ObsType;
    public GameObject Model;
    public GameObject PickUpEffect;
}