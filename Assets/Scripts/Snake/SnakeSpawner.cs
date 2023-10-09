using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_HummPrefab;
    [SerializeField] private float m_SpawnTimer = 1f;


    private float m_Timer = 0f;

    private void Update()
    {
        if (GameController.Instance.GetState != GameState.Game)
            return;

        m_Timer += Time.deltaTime;
        if (m_Timer >= m_SpawnTimer)
        {
            m_Timer = 0f;
            var humm = Instantiate(m_HummPrefab, transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm)
        {
            humm.SetElevatorState(false);
        }
    }
}
