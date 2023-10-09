using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Text m_TestName;
    private ObstacleType m_Type;
    private LayerMask m_DragLayer;
    private Canvas m_MainCanvas;
    private RectTransform m_RectTransform;
    private Vector3 m_DefaultPos;
    private Vector2 m_DragPos;

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_MainCanvas = GetComponentInParent<Canvas>();
        m_DragLayer = DragAndDropController.Instance.GetDragLayer;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DefaultPos = m_DragPos = m_RectTransform.anchoredPosition;
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_DragPos += eventData.delta;

        Vector3 bindPos = BindItem();

        if (bindPos == Vector3.zero)
        {
            transform.localScale = Vector3.one * 1.5f;
            m_RectTransform.anchoredPosition = m_DragPos;
        }
        else
        {
            transform.localScale = Vector3.one;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(bindPos);
            m_RectTransform.anchoredPosition = screenPos - m_MainCanvas.GetComponent<RectTransform>().sizeDelta / 2f - new Vector2(-407.5f / 2f, -337.5f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IModifiable selectedPoint = CheckPoint();
        m_RectTransform.anchoredPosition = m_DefaultPos;
        if (selectedPoint != null && selectedPoint.IsEmpty())
        {
            //selectedPoint.Activate(m_Type, this);
            gameObject.SetActive(false);
        }
        transform.localScale = Vector3.one * 1.5f;
    }

    private Vector3 BindItem()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150f, m_DragLayer))
        {
            return hit.transform.position;
        }
        else
            return Vector2.zero;
    }

    private IModifiable CheckPoint()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150f, m_DragLayer))
        {
            return hit.transform.GetComponentInParent<IModifiable>();
        }

        return null;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void SetData(ObstacleType _type)
    {
        m_Type = _type;

        switch (_type)
        {
            case ObstacleType.Nothing:
                break;
            case ObstacleType.Direction:
                m_TestName.text = "DIR";
                break;
            case ObstacleType.Gravity:
                m_TestName.text = "GRA";
                break;
            case ObstacleType.Accelerator:
                m_TestName.text = "ACC";
                break;
            case ObstacleType.Heaviness:
                m_TestName.text = "HEA";
                break;
            case ObstacleType.Hologram:
                m_TestName.text = "HOL";
                break;
            case ObstacleType.NumberOfTypes:
                break;
            default:
                break;
        }
    }
}
