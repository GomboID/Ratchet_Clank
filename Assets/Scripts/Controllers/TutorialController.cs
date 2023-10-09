using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class TutorialController : Singleton<TutorialController>
{
    [SerializeField] private TutorialData[] m_Data;

    private TutorialData m_CurrentData = new TutorialData();
    private Animator m_Animator;
    private CanvasGroup m_CG;
    private int m_Count = 0;
    private bool m_IsComplete = false;

    private void Start()
    {
        m_CG = GetComponent<CanvasGroup>();
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_GameStart += StartTutor;
        GameController.Instance.Action_GameEnd += EndTutor;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_GameStart -= StartTutor;
        GameController.Instance.Action_GameEnd -= EndTutor;
    }

    private void StartTutor()
    {
        int level = PlayerPrefs.GetInt(Constants.CurrentLevel, 0);

        if (level == 0)
        {
            m_CurrentData = m_Data[0];
            CameraController.Instance.SetScrollState = false;
        }
        else if (level == 1)
        {
            m_CurrentData = m_Data[1];
            CameraController.Instance.SetScrollState = true;
        }
        else if (level == 2)
        {
            m_CurrentData = m_Data[2];
            CameraController.Instance.SetScrollState = false;
        }
        else if (level == 5)
        {
            m_CurrentData = m_Data[3];
            CameraController.Instance.SetScrollState = false;
        }

        if (m_CurrentData.Actions != null)
        {
            m_Animator.SetTrigger(m_CurrentData.AniamtorTrigger);
            m_CurrentData.Container.SetActive(true);

            ShowTutor();
        }
        else
            m_IsComplete = true;
    }

    private void ShowTutor()
    {
        m_CG.DOFade(1f, 0.25f);
    }

    private void EndTutor()
    {
        if (m_CurrentData.IsLastTutorial)
            EventManager.Instance.TutorialComplete();
        m_CG.DOFade(0f, 0.25f);
        m_IsComplete = true;
        CameraController.Instance.SetScrollState = true;
    }

    public void NextStep(TutorType _type)
    {
        if (m_IsComplete)
            return;
        if (m_CurrentData.Actions[m_Count] == _type)
        {
            EventManager.Instance.TutorialStep(m_CurrentData.TutorialName, m_CurrentData.StepName[m_Count]);
            m_Count++;

            if (m_Count >= m_CurrentData.Actions.Length)
                EndTutor();
            else
                m_Animator.SetTrigger("Next");
        }
        else if (m_Count > 0)
        {
            m_Count--;
            m_Animator.SetTrigger("Preview");
        }
    }
}

[Serializable]
public struct TutorialData
{
    public string TutorialName;
    public GameObject Container;
    public string AniamtorTrigger;
    public TutorType[] Actions;
    public string[] StepName;
    public bool IsLastTutorial;
}

public enum TutorType
{
    Nothing,
    Drag,
    Restore,
    PickUp,
    Scroll
}