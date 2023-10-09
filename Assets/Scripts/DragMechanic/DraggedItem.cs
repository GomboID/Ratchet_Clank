using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using TMPro;

public class DraggedItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_Text;

    private ObstacleType m_ObsType;
    private Vector3 m_DefaultPos;
    private Collider m_Collider;
    private Sequence m_Animation;

    public ObstacleType GetObsType => m_ObsType;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
    }

    public void Selected()
    {
        m_Collider.enabled = m_Text.enabled = false;
    }

    public void Deselected()
    {
        m_Animation?.Kill();
        m_Animation = DOTween.Sequence()
            .Append(transform.DOLocalMove(m_DefaultPos, 0.25f).SetEase(Ease.OutFlash))
            .Join(transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutFlash))
            .OnComplete(() =>
            {
                m_Collider.enabled = m_Text.enabled = true;
            });
    }

    public void ApplyModifier(IModifiable _item)
    {
        m_Animation?.Kill();

        m_Animation = DOTween.Sequence()
            .Append(transform.DOScale(0.75f, 0.05f).SetEase(Ease.InFlash))
            .Append(transform.DOScale(new Vector3(1.25f, 1.25f, 2f), 0.15f).SetEase(Ease.OutFlash))
            .Append(transform.DOScale(0.5f, 0.15f).SetEase(Ease.InFlash))
            .Join(transform.DOMove(transform.position - Vector3.up * 2f, 0.15f).SetEase(Ease.InFlash))
            .OnKill(()=> 
            {
                gameObject.SetActive(false);
                _item.Activate(m_ObsType, this);
                DragAndDropController.Instance.AlignElements();
                GamePlayUI.Instance.ReduceAttempts();
            });
    }

    public void RestoreMultiplier()
    {
        gameObject.SetActive(true);
        m_Animation?.Kill();

        m_Animation = DOTween.Sequence()
            .Append(transform.DOScale(1.25f, 0.15f).SetEase(Ease.InFlash))
            .Join(transform.DOMove(transform.position + Vector3.up * 2f, 0.15f).SetEase(Ease.InFlash))
            .Append(transform.DOScale(new Vector3(0.75f, 0.75f, 2f), 0.1f).SetEase(Ease.OutFlash))
            .Append(transform.DOScale(1f, 0.1f).SetEase(Ease.InFlash))
            .OnComplete(() =>
            {
                m_Animation = DOTween.Sequence()
                .Append(transform.DOLocalMove(m_DefaultPos, 0.25f).SetEase(Ease.OutFlash))
                .Join(transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutFlash))
                .OnComplete(() =>
                {                    
                    DragAndDropController.Instance.AlignElements();                    
                    m_Collider.enabled = m_Text.enabled = true;
                });
            });
    }   

    public void SetData(ObstacleType _type)
    {
        m_ObsType = _type;
        Instantiate(DataController.Instance.GetDragIcon(m_ObsType), transform);

        switch (_type)
        {
            case ObstacleType.Nothing:
                m_Text.text = string.Empty;
                break;
            case ObstacleType.Direction:
                m_Text.text = "ROTATE";
                break;
            case ObstacleType.Gravity:
                m_Text.text = "GRAVITY";
                break;
            case ObstacleType.Accelerator:
                m_Text.text = "SPEED";
                break;
            case ObstacleType.Heaviness:
                m_Text.text = "HEAVY";
                break;
            case ObstacleType.Hologram:
                m_Text.text = "HOLOGRAM";
                break;
            case ObstacleType.NumberOfTypes:
                m_Text.text = string.Empty;
                break;
            default:
                break;
        }
    }

    public void SetDefaultPos(Vector3 _pos)
    {
        m_DefaultPos = _pos;
        transform.localPosition = _pos;
    }
}

