using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class RotatableRoad : MonoBehaviour, IModifiable
{
    [SerializeField] private Transform m_RotateTrans;
    [SerializeField] private Vector3 m_ActivatedRotation;
    [SerializeField] private GameObject m_DefaultIcon, m_DragIcon;
    [SerializeField] private ModifierModel[] m_ModModels;

    private Vector3 m_DefaultRotation;
    private DraggedItem m_DraggetItem;
    private Sequence m_Animation;

    private void Awake()
    {
        m_DefaultRotation = m_RotateTrans.eulerAngles;
    }

    public void Activate(ObstacleType _obsType, DraggedItem _item)
    {
        m_DraggetItem = _item;
        ShowModel(m_ModModels.FirstOrDefault(a => a.ObsType == _obsType).ModModel.transform);
        if (_obsType == ObstacleType.Direction)
        {           
            m_DefaultIcon.gameObject.SetActive(false);
            m_Animation?.Kill();
            m_Animation = DOTween.Sequence()
                .Append(m_RotateTrans.DORotate(m_ActivatedRotation, 0.5f).SetEase(Ease.Linear));
        }
    }

    public void Deactivate()
    {
        if (!m_DraggetItem)
            return;
        m_DefaultIcon.gameObject.SetActive(true);
        HideModel(m_ModModels.FirstOrDefault(a => a.ObsType == m_DraggetItem.GetObsType).ModModel.transform);
        ResetModifier();

        m_Animation?.Kill();
        m_Animation = DOTween.Sequence()
            .Append(m_RotateTrans.DORotate(m_DefaultRotation, 0.5f).SetEase(Ease.Linear));
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

    public bool IsEmpty()
    {
        return m_DraggetItem == null;
    }

    public void SetDragIconState(bool _isActive)
    {
        m_DragIcon.SetActive(_isActive);
    }
}
