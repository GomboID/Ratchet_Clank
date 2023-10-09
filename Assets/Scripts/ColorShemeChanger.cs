using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorShemeChanger : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] m_MR;

    private void Start()
    {
        Material mat = DataController.Instance.GetCurrentCheme.MainMat;

        for (int i = 0; i < m_MR.Length; i++)
        {
            m_MR[i].material = mat;
        }
    }
}
