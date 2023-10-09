using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterItem : MonoBehaviour
{
    [SerializeField] private bool m_IsBonus = false;
    [SerializeField] private GameObject m_BonusContainer, m_DefaultContainer, m_LineObject;
    [SerializeField] private Text m_IndexText;
    [SerializeField] private Image m_Background, m_CompleteBackground, m_CurrentBackground;

    private void Awake()
    {
        m_BonusContainer.SetActive(m_IsBonus);
        m_DefaultContainer.SetActive(!m_IsBonus);
    }

    public void SetState(int _index, int _level)
    {
        m_IndexText.text = (_index + 1).ToString();

        if (_index < _level)
        {
            m_IndexText.text = string.Empty;
            m_LineObject.SetActive(true);
            m_CompleteBackground.enabled = true;
        }
        else if (_index == _level)
        {
            m_CurrentBackground.enabled = true;
        }
    }
}
