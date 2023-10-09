using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private DynamicJoystick m_Joystick;

    private Vector3 m_PreviewFingerPos;
    private Camera m_Camera;
    private bool m_IsScrollActive = true, m_IsScrolling = false;
    private float m_ScrollDelay = 1f;
    private Sequence m_DelaySeq, m_MoveAnimation;
    private Vector3 m_FinishPos, m_CameraDefaultPos;

    public bool SetScrollState
    {
        set
        {
            m_IsScrollActive = value;
        }
    }

    public bool IsScrolling => m_IsScrolling;

    protected override void Awake()
    {
        base.Awake();
        m_Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        m_FinishPos = LevelController.Instance.GetLevel.GetFinishPosition;
        m_CameraDefaultPos = LevelController.Instance.GetLevel.GetCameraPos;
        transform.position = GetFinishPos();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_GameStart += GameStart;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_GameStart -= GameStart;
    }

    private void LateUpdate()
    {
        if (GameController.Instance.GetState != GameState.Game || !m_IsScrollActive || !DragAndDropController.Instance.IsItemSelected)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            m_PreviewFingerPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            m_DelaySeq?.Kill();
            m_DelaySeq = DOTween.Sequence()
                .AppendInterval(m_ScrollDelay)
                .OnUpdate(()=> 
                {
                    float delta = Vector3.Distance(m_PreviewFingerPos, m_Camera.ScreenToWorldPoint(Input.mousePosition));
                    if (delta > 1f)
                    {
                        m_DelaySeq?.Kill();
                        m_IsScrolling = true;
                        m_PreviewFingerPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
                        TutorialController.Instance.NextStep(TutorType.Scroll);
                    }
                })
                .OnComplete(() => 
                {
                    m_IsScrolling = true;
                    m_PreviewFingerPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
                    TutorialController.Instance.NextStep(TutorType.Scroll);
                });            
        }

        if (Input.GetMouseButton(0) && m_Joystick.Direction.magnitude > 0.1f && m_IsScrolling)
        {
            m_MoveAnimation?.Kill();
            Vector3 delta = m_PreviewFingerPos - m_Camera.ScreenToWorldPoint(Input.mousePosition);
            delta.y = 0;
            m_Camera.transform.position = m_Camera.transform.position + delta * 0.65f;

            Vector3 pos = m_Camera.transform.position;
            pos.x = Mathf.Clamp(pos.x, 5f, 12f);
            float maxZ = m_FinishPos.z - 17f;
            maxZ = maxZ < -3 ? -3f : maxZ;
            pos.z = Mathf.Clamp(pos.z, -5f, maxZ);

            m_Camera.transform.position = pos;

            m_PreviewFingerPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            m_IsScrolling = false;
            m_DelaySeq?.Kill();
        }
    }

    private void GameStart()
    {
        float distance = Vector3.Distance(m_CameraDefaultPos, transform.position);
        if (distance > 2)
        {
            float timer = distance / 5;
            timer = Mathf.Clamp(timer, 0.5f, 2f);
            m_MoveAnimation = DOTween.Sequence()
                .Append(transform.DOMove(m_CameraDefaultPos, timer).SetEase(Ease.OutFlash));
        }        
    }

    public void LookOnFinish()
    {
        m_MoveAnimation = DOTween.Sequence()
            .Append(transform.DOMove(GetFinishPos(), 0.5f).SetEase(Ease.InFlash));
    }

    private Vector3 GetFinishPos()
    {        
        Vector3 newPos = m_CameraDefaultPos;
        newPos.z = m_FinishPos.z - 17f;
        newPos.x += m_FinishPos.x;
        return newPos;
    }
}
