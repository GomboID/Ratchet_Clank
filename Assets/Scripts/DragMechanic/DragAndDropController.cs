using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class DragAndDropController : Singleton<DragAndDropController>
{
    [SerializeField] private LayerMask m_DragTriggerLayer, m_DragLayer;
    [SerializeField] private Transform m_DragItem, m_ItemsContainer;

    private List<DraggedItem> m_Items = new List<DraggedItem>();
    private DraggedItem m_SelectedItem;
    private Vector3 m_DefaultPos;
    private IModifiable m_BindedItem;

    public LayerMask GetDragLayer => m_DragTriggerLayer;

    public bool IsItemSelected => m_SelectedItem == null;

    protected override void Awake()
    {
        base.Awake();
        m_DefaultPos = transform.localPosition;
        transform.localPosition = m_DefaultPos - Vector3.up * 15f;
    }

    private void Start()
    {
        CreateUI();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_GameStart += GameStart;
        GameController.Instance.Action_GameEnd += GameEnd;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_GameStart -= GameStart;
        GameController.Instance.Action_GameEnd -= GameEnd;
    }

    private void GameStart()
    {
        transform.DOLocalMove(m_DefaultPos, 0.25f);
    }

    private void GameEnd()
    {
        transform.DOLocalMove(m_DefaultPos - Vector3.up * 15f, 0.25f);
    }

    private void CreateUI()
    {
        ObstacleType[] startTypes = LevelController.Instance.GetLevel.GetStartModifiers;

        for (int i = 0; i < startTypes.Length; i++)
        {
            CreateNewModifier(startTypes[i]);
        }
    }

    private void Update()
    {
        if (GameController.Instance.GetState != GameState.Game)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 50f, m_DragTriggerLayer))
            {
                DraggedItem item = hit.transform.GetComponentInParent<DraggedItem>();

                if (item)
                {
                    m_SelectedItem = item;
                    m_SelectedItem.Selected();                    
                }
            }
        }

        if (Input.GetMouseButton(0) && m_SelectedItem)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.up * 135f);


            if (Physics.Raycast(ray, out hit, 50f, m_DragTriggerLayer))
            {
                if (m_BindedItem != null)
                {
                    m_BindedItem.SetDragIconState(false);
                    m_BindedItem = null;
                }

                m_BindedItem = hit.transform.GetComponentInParent<IModifiable>();

                if (m_BindedItem != null && m_BindedItem.IsEmpty())
                {
                    m_SelectedItem.transform.position = Vector3.Lerp(m_SelectedItem.transform.position, hit.transform.position + Vector3.up * 4f, Time.deltaTime * 10f);
                    m_SelectedItem.transform.rotation = Quaternion.Lerp(m_SelectedItem.transform.rotation, Quaternion.Euler(Vector3.right * 90f), Time.deltaTime * 10f);
                    m_BindedItem.SetDragIconState(true);
                }
                else
                    DragOnPlane();
            }
            else
                DragOnPlane();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_SelectedItem)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition + Vector3.up * 135f);

                if (Physics.Raycast(ray, out hit, 50f, m_DragTriggerLayer))
                {
                    m_BindedItem = hit.transform.GetComponentInParent<IModifiable>();

                    if (m_BindedItem != null && m_BindedItem.IsEmpty())
                    {
                        m_BindedItem.SetDragIconState(false);
                        m_SelectedItem.ApplyModifier(m_BindedItem);
                        TutorialController.Instance.NextStep(TutorType.Drag);
                    }
                    else
                    {
                        m_SelectedItem.RestoreMultiplier();
                    }
                }
                else
                {
                    m_SelectedItem.Deselected();
                }
                m_SelectedItem = null;
            }
            else if (!CameraController.Instance.IsScrolling)
            {   
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray, 50f, m_DragTriggerLayer);

                foreach (var item in hits)
                {
                    IModifiable obs = item.transform.GetComponentInParent<IModifiable>();
                    if (obs != null && !obs.IsEmpty())
                    {
                        obs.Deactivate();
                        break;
                    }
                }
            }
        }
    }

    private void DragOnPlane()
    {
        if (m_BindedItem != null)
        {
            m_BindedItem.SetDragIconState(false);
            m_BindedItem = null;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 50f, m_DragLayer))
        {
            m_SelectedItem.transform.position = Vector3.Lerp(m_SelectedItem.transform.position, hit.point + m_SelectedItem.transform.TransformDirection(Vector3.up) * 3f, Time.deltaTime * 10f);
            m_SelectedItem.transform.localRotation = Quaternion.Lerp(m_SelectedItem.transform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
        }
    }

    public void CreateNewModifier(ObstacleType _newObs)
    {
        DraggedItem newItem = Instantiate(m_DragItem, m_ItemsContainer).GetComponent<DraggedItem>();
        newItem.SetData(_newObs);
        m_Items.Add(newItem);
        AlignElements();
    }

    public DraggedItem CreateDefaultModifier(ObstacleType _newObs)
    {
        CreateNewModifier(_newObs);
        return m_Items.Last();
    }

    public void AlignElements()
    {
        List<DraggedItem> activeItems = m_Items.Where(a => a.gameObject.activeSelf).ToList();

        for (int i = 0; i < activeItems.Count; i++)
        {
            activeItems[i].SetDefaultPos(new Vector3(3.5f * (i - activeItems.Count / 2) - (activeItems.Count % 2 - 1) * 1.75f, 0f, 0f));
        }
    }
}
