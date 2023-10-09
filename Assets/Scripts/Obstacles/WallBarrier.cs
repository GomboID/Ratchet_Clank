using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBarrier : MonoBehaviour
{
    [SerializeField] private bool m_IsBreakble = false;
    [SerializeField] private int m_Health = 5;
    [SerializeField] private GameObject m_FullObject, m_PiecesObj;
    [SerializeField] private Rigidbody[] m_WallPieces;
    [SerializeField] private ParticleSystem m_HitEffect;

    private Collider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm)
        {
            m_IsBreakble = m_WallPieces.Length > 0;

            if (m_IsBreakble)
            {
                if (humm.IsAccelerated)
                {
                    m_Health--;
                }
                humm.Death();
                if (m_Health <= 0)
                {
                    //активируем куски
                    m_FullObject.SetActive(false);
                    m_PiecesObj.SetActive(true);
                    m_Collider.enabled = false;

                    for (int i = 0; i < m_WallPieces.Length; i++)
                    {
                        m_WallPieces[i].isKinematic = false;
                        m_WallPieces[i].useGravity = true;
                        m_WallPieces[i].AddExplosionForce(3f, transform.position, 2f, 1f, ForceMode.Impulse);
                        m_WallPieces[i].AddTorque(new Vector3(Random.Range(-45f, 45f), Random.Range(-45f, 45f), Random.Range(-45f, 45f)), ForceMode.Impulse);
                        Destroy(m_WallPieces[i].gameObject, 1f);
                    }


                }
            }
            else if (humm.GetCurrentState != ObstacleType.Hologram)
            {
                m_HitEffect.Play();
                humm.Death();
            }
        }
    }
}
