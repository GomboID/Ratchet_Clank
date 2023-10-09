using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratorScript : MonoBehaviour
{
    private MeshRenderer m_MR;
    private Vector2 m_Offset = Vector2.zero;

    private void Awake()
    {
        m_MR = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        m_Offset.y -= Time.deltaTime / 3f;
        m_MR.material.SetTextureOffset("_BaseMap", m_Offset);
    }
}
