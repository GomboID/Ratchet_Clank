using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;

public class GravityBarrier : MonoBehaviour, IModifiable
{
    [SerializeField] private GravityState[] m_States;
    [SerializeField] private Transform m_Model;
    [SerializeField] private BoxCollider m_GroundCollider;
    [SerializeField] private GameObject m_DefaultIcon, m_DragIcon;
    [SerializeField] private ModifierModel[] m_ModModels;
    [SerializeField] private BoxCollider m_RestoreCollider;

    private DraggedItem m_DraggetItem;
    private GravityState m_CurrentState;
    private Sequence m_Animation;
    private Vector3 m_DefaultPosition;
    private Collider m_Collider;

    private void Awake()
    {
        m_DefaultPosition = m_Model.position;
        m_Collider = GetComponent<Collider>();
        m_CurrentState = m_States.FirstOrDefault(a => a.ObsType == ObstacleType.Nothing);
    }

    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm)
        {
            humm.Death();
        }
    }

    public void Activate(ObstacleType _obsType, DraggedItem _item)
    {
        m_DraggetItem = _item;
        ShowModel(m_ModModels.FirstOrDefault(a => a.ObsType == _obsType).ModModel.transform);
        if (_obsType == ObstacleType.Gravity || _obsType == ObstacleType.Heaviness)
        {
            m_CurrentState = m_States.FirstOrDefault(a => a.ObsType == _obsType);
            m_DefaultIcon.SetActive(false);
            m_RestoreCollider.enabled = true;
            SetState();
        }
    }

    public void Deactivate()
    {
        if (!m_DraggetItem)
            return;

        m_DefaultIcon.SetActive(true);
        if (m_CurrentState.Effect)
            m_CurrentState.Effect.SetActive(false);
        m_CurrentState = m_States.FirstOrDefault(a => a.ObsType == ObstacleType.Nothing);
        SetState();

        m_DraggetItem.RestoreMultiplier();

        m_ModModels.FirstOrDefault(a => a.ObsType == m_DraggetItem.GetObsType).ModModel.SetActive(false);
        HideModel(m_ModModels.FirstOrDefault(a => a.ObsType == m_DraggetItem.GetObsType).ModModel.transform);

        m_CurrentState = null;
        m_RestoreCollider.enabled = false;
        ResetModifier();
    }

    private void ShowModel(Transform _trans)
    {
        _trans.transform.localScale = Vector3.zero;
        _trans.gameObject.SetActive(true);

        Sequence anim = DOTween.Sequence()
            .Append(_trans.DOScale(1.15f, 0.15f).SetEase(Ease.InFlash))
            .Append(_trans.DOScale(0.9f, 0.1f).SetEase(Ease.OutFlash))
            .Append(_trans.DOScale(0.9f, 0.05f).SetEase(Ease.InFlash));
    }

    private void HideModel(Transform _trans)
    {
        _trans.transform.localScale = Vector3.one;
        _trans.gameObject.SetActive(true);

        Sequence anim = DOTween.Sequence()
            .Append(_trans.DOScale(1.15f, 0.15f).SetEase(Ease.InFlash))
            .Append(_trans.DOScale(0f, 0.1f).SetEase(Ease.OutFlash))
            .OnComplete(() =>
            {
                _trans.gameObject.SetActive(false);
            });
    }

    private void ResetModifier()
    {
        m_DraggetItem.transform.position = transform.position + Vector3.up * 0.5f;
        m_DraggetItem.transform.localScale = Vector3.one * 0.5f;
        m_DraggetItem.transform.rotation = Quaternion.Euler(Vector3.right * 90f);
        m_DraggetItem.RestoreMultiplier();
        m_DraggetItem = null;
    }

    private void SetState()
    {
        m_Collider.enabled = m_CurrentState.IsKillHummanoids;
        m_GroundCollider.enabled = m_CurrentState.IsActivateGround;
        if (m_CurrentState.Effect)
            m_CurrentState.Effect.SetActive(true);
        m_Animation?.Kill();
        m_Animation = DOTween.Sequence()
            .Append(m_Model.DOMove(m_DefaultPosition + m_CurrentState.PosOffset, 0.25f));
    }

    public bool IsEmpty()
    {
        return m_DraggetItem == null;
    }

    public void SetDragIconState(bool _isActive)
    {
        m_DragIcon.SetActive(_isActive);
    }
}

[Serializable]
public class GravityState
{
    public ObstacleType ObsType;
    public Vector3 PosOffset;
    public bool IsKillHummanoids;
    public bool IsActivateGround;
    public GameObject Effect;
}

[Serializable]
public struct ModifierModel
{
    public ObstacleType ObsType;
    public GameObject ModModel;
}
