using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GamePlayUI : Singleton<GamePlayUI>
{
    [SerializeField] private GameObject m_AttemptPrefab;
    [SerializeField] private Transform m_AttemptsContainer;
    [SerializeField] private Text m_LevelText;

    private CanvasGroup m_CG;
    private Sequence m_Animation;
    private int m_Attempts = 3;
    private List<AttemptItem> m_AttemptsList = new List<AttemptItem>();

    public int GetAttempts => m_Attempts;

    protected override void Awake()
    {
        base.Awake();
        m_CG = GetComponent<CanvasGroup>();
        m_LevelText.text = "LEVEL " + (PlayerPrefs.GetInt(Constants.CurrentLevel, 0) + 1).ToString();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_GameStart += ShowUI;
        GameController.Instance.Action_GameEnd += HideUI;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_GameStart -= ShowUI;
        GameController.Instance.Action_GameEnd -= HideUI;
    }

    private void ShowUI()
    {
        CreateAttemptsUI();
        m_Animation?.Kill();
        m_Animation = DOTween.Sequence()
            .Append(m_CG.DOFade(1f, 0.1f))
            .OnComplete(()=> 
            {
                m_CG.blocksRaycasts = true;
            });
    }

    private void CreateAttemptsUI()
    {
        m_Attempts += LevelController.Instance.GetLevel.GetAttemptsCount;

        for (int i = 0; i < m_Attempts; i++)
        {
            AttemptItem item = Instantiate(m_AttemptPrefab, m_AttemptsContainer).GetComponent<AttemptItem>();
            item.SetState(i, m_Attempts);
            m_AttemptsList.Add(item);
        }
    }

    private void UpdateAttemptState()
    {       
        for (int i = 0; i < m_AttemptsList.Count; i++)
        {
            m_AttemptsList[i].SetState(i, m_Attempts);
        }

        if (m_Attempts <= 0)
            GameController.Instance.Lose();
    }

    public void ReduceAttempts()
    {
        m_Attempts--;
        UpdateAttemptState();
    }

    private void HideUI()
    {
        m_CG.blocksRaycasts = false;
        m_Animation?.Kill();
        m_Animation = DOTween.Sequence()
            .Append(m_CG.DOFade(0f, 0.1f));
    }
}
