using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private Button m_RetryButton;
    [SerializeField] private Text m_LevelText;
    private CanvasGroup m_CG;

    private void Awake()
    {
        m_CG = GetComponent<CanvasGroup>();
        m_RetryButton.onClick.AddListener(RetryButtonClick);
        m_LevelText.text = "LEVEL " + (PlayerPrefs.GetInt(Constants.CurrentLevel, 0) + 1).ToString();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_Lose += ShowUI;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_Lose -= ShowUI;
    }

    private void ShowUI()
    {
        Sequence seq = DOTween.Sequence()
            .AppendInterval(1.5f)
            .Append(m_CG.DOFade(1f, 0.25f))
            .OnComplete(() =>
            {
                m_CG.blocksRaycasts = true;
            });
    }

    private void RetryButtonClick()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(Constants.GameScene);
    }
}
