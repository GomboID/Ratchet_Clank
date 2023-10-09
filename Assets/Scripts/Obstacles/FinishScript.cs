using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] m_Buttons;
    [SerializeField] private ParticleSystem[] m_Effects;
    [SerializeField] private ParticleSystem[] m_ConfetiEffects;
    private BoxCollider m_Collider;
    private int m_FinisHummCount = 0;

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm)
        {
            if (m_FinisHummCount < m_Buttons.Length)
            {
                m_Buttons[m_FinisHummCount].material = DataController.Instance.GetCurrentCheme.ActiveButtons;
                m_Effects[m_FinisHummCount].Play();

            }
            else if (m_FinisHummCount == m_Buttons.Length)
            {
                foreach (var item in m_ConfetiEffects)
                {
                    item.Play();
                }
                GameController.Instance.Win();
            }

            m_FinisHummCount++;

            if (m_FinisHummCount == 1)
                CameraController.Instance.LookOnFinish();

            humm.SetElevatorState(true);
        }
    }
}
