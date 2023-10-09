using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using System.Linq;

public class ObstacleScript : MonoBehaviour, IModifiable
{
    [SerializeField] private ObsState[] m_States;
    [SerializeField] private GameObject m_DefaultIcon, m_DragIcon;
    [SerializeField] private ObstacleType m_DefaultType = ObstacleType.Nothing;

    private ObsState m_CurrentState;
    private DraggedItem m_DraggetItem;
    private Sequence m_Animation;

    private void Start()
    {
        if (m_DefaultType != ObstacleType.Nothing)
        {
            m_DraggetItem = DragAndDropController.Instance.CreateDefaultModifier(m_DefaultType);
            m_DraggetItem.gameObject.SetActive(false);
            m_DraggetItem.Selected();

            Activate(m_DefaultType, m_DraggetItem);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        SnakeHummanoid humm = other.GetComponent<SnakeHummanoid>();

        if (humm && m_CurrentState != null)
        {        
            switch (m_CurrentState.ObsType)
            {
                case ObstacleType.Direction:
                    humm.ChangeMoveDirection(m_CurrentState.ObjToRotate.forward, Vector3.Distance(transform.position, humm.transform.position));
                    break;
                case ObstacleType.Gravity:
                    humm.Jump();
                    break;
                case ObstacleType.Accelerator:
                    humm.Accelerate();
                    break;
                case ObstacleType.Hologram:
                    humm.SetHologram();
                    break;
                case ObstacleType.Heaviness:
                    humm.SetHeaviness();
                    break;
                default:
                    break;
            }

            if (m_CurrentState.Effect != null)
                m_CurrentState.Effect.Play();
        }
    }

    public void SetDragIconState(bool _isActive)
    {
        m_DragIcon.SetActive(_isActive);
    }

    public void Activate(ObstacleType _obsType, DraggedItem _item)
    {
        m_CurrentState = m_States.FirstOrDefault(a => a.ObsType == _obsType);
        if (m_CurrentState != null)
        {
            m_DraggetItem = _item;
            m_DefaultIcon.SetActive(false);
            ShowModel(m_CurrentState.ObjectToActivate);

            if (_obsType == ObstacleType.Direction)
            {
                m_Animation?.Kill();
                m_Animation = DOTween.Sequence()
                    .Append(m_CurrentState.ObjToRotate.DORotate(Vector3.up * m_CurrentState.RotateTo, 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
            }
        }
    }

    public void Deactivate()
    {
        if (m_CurrentState == null)
            return;

        m_DefaultIcon.SetActive(true);
        HideModel(m_CurrentState.ObjectToActivate);

        if (m_CurrentState.ObsType == ObstacleType.Direction)
        {
            m_Animation?.Kill();
            m_Animation = DOTween.Sequence()
                .Append(m_CurrentState.ObjToRotate.DORotate(-Vector3.up * m_CurrentState.RotateTo, 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear));
        }

        m_CurrentState = null;
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
            .OnComplete(()=> 
            {
                _trans.gameObject.SetActive(false);
            });
    }

    private void ResetModifier()
    {
        TutorialController.Instance.NextStep(TutorType.Restore);
        m_DraggetItem.transform.position = transform.position + Vector3.up * 0.5f;
        m_DraggetItem.transform.localScale = Vector3.one * 0.5f;
        m_DraggetItem.transform.rotation = Quaternion.Euler(Vector3.right * 90f);
        m_DraggetItem.RestoreMultiplier();
        m_DraggetItem = null;
    }

    public bool IsEmpty()
    {
        return m_DraggetItem == null;
    }
}


[Serializable]
public class ObsState
{
    public ObstacleType ObsType;
    public Transform ObjectToActivate;
    public Transform ObjToRotate;
    public ParticleSystem Effect;
    public float RotateTo;
}

public interface IModifiable
{
    public void Activate(ObstacleType _obsType, DraggedItem _item);
    public void Deactivate();
    public bool IsEmpty();
    public void SetDragIconState(bool _isActive);
}
