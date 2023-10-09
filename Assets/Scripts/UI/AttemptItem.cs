using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttemptItem : MonoBehaviour
{
    [SerializeField] private GameObject m_BonusIcon, m_ActiveIcon;

    public void SetState(int _index, int _count)
    {
        if (_index < _count)
        {
            if (_index < 3)
                m_BonusIcon.SetActive(true);
            else
                m_ActiveIcon.SetActive(true);
        }
        else
        {
            m_BonusIcon.SetActive(false);
            m_ActiveIcon.SetActive(false);
        }
    }
}
