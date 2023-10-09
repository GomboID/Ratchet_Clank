using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class WinUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_StarFlyPoint, m_GlowImage;
    [SerializeField] private Button m_NextButton, m_RewardButton;
    [SerializeField] private Text m_LevelText, m_ChapterText, m_ProgressText, m_RewardedStars;
    [SerializeField] private Image m_StarProgress;
    [SerializeField] private ChapterItem[] m_Chapters;
    [SerializeField] private StarStr[] m_Stars;

    private Sequence m_RewardSequence;
    private int m_StarsCount = 0, m_BonusStars = 0, m_RewardStars = 3, m_StarsToOpen;
    private CanvasGroup m_CG;
    

    private void Awake()
    {        
        m_CG = GetComponent<CanvasGroup>();
        m_NextButton.onClick.AddListener(NextButtonClick);
        m_RewardButton.onClick.AddListener(RewardButtonClick);
        m_LevelText.text = "LEVEL " + (PlayerPrefs.GetInt(Constants.CurrentLevel, 0) + 1).ToString();
        m_StarsCount = PlayerPrefs.GetInt(Constants.StarsCount, 0);
    }

    private void Start()
    {
        m_RewardStars = StatsController.Instance.GetCompleteLevels % 4 == 0 ? 5 : 3;
        m_RewardedStars.text = m_RewardStars.ToString();
        m_StarsToOpen = DataController.Instance.GetStarsCount;
    }

    private void OnEnable()
    {
        GameController.Instance.Action_Win += ShowUI;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_Win -= ShowUI;
    }

    private void ShowUI()
    {
        SetState();
        Sequence rewardSeq = DOTween.Sequence()
            .AppendInterval(2f)
            .Append(m_RewardButton.transform.DOScale(0.85f, 0.1f).SetEase(Ease.InFlash))
            .Append(m_RewardButton.transform.DOScale(1.15f, 0.15f).SetEase(Ease.OutFlash))
            .Append(m_RewardButton.transform.DOScale(1f, 0.05f).SetEase(Ease.InFlash));

        Sequence glowSeq = DOTween.Sequence()
            .AppendInterval(1f)
            .Append(m_GlowImage.DOAnchorPosX(m_GlowImage.anchoredPosition.x * -1, 0.5f).SetEase(Ease.Linear));

        m_RewardSequence = DOTween.Sequence()
            .AppendInterval(2f)
            .Append(rewardSeq)
            .Join(glowSeq)
            .SetLoops(-1, LoopType.Restart)
            .OnKill(()=> 
            { 
                m_RewardButton.transform.localScale = Vector3.one;
                m_GlowImage.gameObject.SetActive(false);
            });
        Sequence seq = DOTween.Sequence()
            .AppendInterval(1.5f)
            .Append(m_CG.DOFade(1f, 0.25f));
    }

    private void SetState()
    {
        int currentLevel = PlayerPrefs.GetInt(Constants.CurrentLevel, 0);
        int convertedIndex = currentLevel - (int)(currentLevel / m_Chapters.Length) * m_Chapters.Length;

        for (int i = 0; i < m_Chapters.Length; i++)
        {
            m_Chapters[i].SetState(i, convertedIndex);
        }

        m_BonusStars = Mathf.Clamp(GamePlayUI.Instance.GetAttempts, 0, 3);
        
        m_ChapterText.text = "CHAPTER " + (currentLevel / 7).ToString();
        m_StarProgress.fillAmount = (float)m_StarsCount / m_StarsToOpen;
        m_ProgressText.text = m_StarsCount + "/" + m_StarsToOpen.ToString();

        Sequence starAnimation = DOTween.Sequence()
            .AppendInterval(2f)
            .OnComplete(()=> 
            {
                starAnimation = DOTween.Sequence();
                for (int i = 0; i < m_BonusStars; i++)
                {
                    int imdex = i;

                    Sequence timer = DOTween.Sequence()
                    .AppendInterval(0.05f * i)
                    .OnComplete(() => 
                    {
                        m_Stars[imdex].StarCont.SetActive(true);
                    });

                    Sequence anim = DOTween.Sequence()
                    .Append(m_Stars[i].StarTransform.DOScale(1.25f, 0.1f).SetEase(Ease.OutFlash))
                    .Append(m_Stars[i].StarTransform.DOScale(1f, 0.15f).SetEase(Ease.InFlash));

                    starAnimation
                    .Append(timer)
                    .Append(anim);
                }

                starAnimation.OnComplete(() =>
                {
                    m_CG.blocksRaycasts = true;
                    m_NextButton.gameObject.SetActive(true);
                    EventManager.Instance.RewardOffer(Constants.Reward_BonusStars);
                });
            });     
    }

    private void RewardButtonClick()
    {
        m_RewardSequence.Kill();
        ADSManager.Instance.Action_IsRewarded += RewardPlayer;
        ADSManager.Instance.ShowRewarded();        
    }

    public void RewardPlayer(bool _isRewarded)
    {
        ADSManager.Instance.Action_IsRewarded -= RewardPlayer;
        if (_isRewarded)
        {
            m_StarsCount += m_RewardStars;
            StatsController.Instance.ResetLevel();
        }
        NextButtonClick();
    }

    private void NextButtonClick()
    {
        Sequence starAnimation = DOTween.Sequence();
        m_CG.blocksRaycasts = false;
        for (int i = 0; i < m_BonusStars; i++)
        {
            int imdex = i;

            Sequence timer = DOTween.Sequence()
            .AppendInterval(0.05f * i)
            .OnComplete(() =>
            {
                m_Stars[imdex].StarCont.SetActive(true);
            });

            Sequence anim = DOTween.Sequence()
            .Append(m_Stars[i].StarTransform.DOScale(0.45f, 0.2f).SetEase(Ease.InFlash))
            .Join(m_Stars[i].StarTransform.DOMove(m_StarFlyPoint.transform.position, 0.2f).SetEase(Ease.InFlash))
            .OnComplete(()=> 
            {
                m_StarsCount++;
                m_StarsCount = Mathf.Clamp(m_StarsCount, 0, m_StarsToOpen);
                m_StarProgress.fillAmount = (float)m_StarsCount / m_StarsToOpen;
                m_ProgressText.text = m_StarsCount + "/" + m_StarsToOpen.ToString();
            });

            starAnimation
            .Append(timer)
            .Append(anim);
        }

        starAnimation
            .AppendInterval(1f)
            .OnComplete(() =>
            {
                PlayerPrefs.SetInt(Constants.CurrentLevel, PlayerPrefs.GetInt(Constants.CurrentLevel, 0) + 1);
                if (m_StarsCount >= m_StarsToOpen)
                {
                    m_StarsCount = 0;
                    PlayerPrefs.SetInt(Constants.LocationCount, PlayerPrefs.GetInt(Constants.LocationCount, 0) + 1);
                }
                PlayerPrefs.SetInt(Constants.StarsCount, m_StarsCount);
                DOTween.KillAll();
                SceneManager.LoadScene(Constants.GameScene);
            });
    }
}

[Serializable]
public struct StarStr
{
    public GameObject StarCont;
    public RectTransform StarTransform;
    public ParticleSystem Effect;
}
