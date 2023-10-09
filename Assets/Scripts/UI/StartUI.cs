using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Button m_StartButton;
    [SerializeField] private Transform m_TextTransform;

    private CanvasGroup m_CG;

    private void Awake()
    {
        m_CG = GetComponent<CanvasGroup>();

        Sequence seq = DOTween.Sequence()
            .AppendInterval(3f)
            .Append(m_TextTransform.DOScale(1.2f, 0.15f).SetEase(Ease.InFlash))
            .Append(m_TextTransform.DOScale(0.9f, 0.1f).SetEase(Ease.OutFlash))
            .Append(m_TextTransform.DOScale(1f, 0.05f).SetEase(Ease.InFlash))
            .SetLoops(-1, LoopType.Restart);
    }

    private void Start()
    {
        m_StartButton.onClick.AddListener(StartButtonClick);
    }

    private void StartButtonClick()
    {
        m_CG.blocksRaycasts = false;

        m_CG.DOFade(0f, 0.25f)
            .OnComplete(() => 
            {
                GameController.Instance.GameStart();
            });
    }
}
