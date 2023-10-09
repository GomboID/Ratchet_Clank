using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnakeHummanoid : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f, m_Gravity = -9.8f, m_JumpHeight = 8f;
    [SerializeField] private DeathPart[] m_HummanoidParts;
    [SerializeField] private SkinnedMeshRenderer m_SMR;
    [SerializeField] private GameObject m_MagnetObject;
    [SerializeField] private ParticleSystem m_AccelerationEffect, m_JumpEffect;

    private Vector3 m_MoveDirection;
    private CharacterController m_CC;
    private Sequence m_StateTimer;
    private Animator m_Animator;
    
    private bool m_IsJump = false, m_IsGroundeed = false, m_IsElevator = true;
    private float m_VerticlaSpeed = 0f;
    private float m_CurrentSpeed, m_FallTimer = 0.15f;
    private ObstacleType m_CurrentState = ObstacleType.Nothing;
    public ObstacleType GetCurrentState => m_CurrentState;
    public bool IsAccelerated => m_CurrentSpeed > m_Speed;

    private void Awake()
    {
        m_CC = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_MoveDirection = Vector3.forward;
        m_CurrentSpeed = m_Speed;
    }

    private void OnEnable()
    {
        GameController.Instance.Action_Lose += Death;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_Lose -= Death;
    }

    private void FixedUpdate()
    {
        transform.LookAt(transform.position + m_MoveDirection);

        m_IsGroundeed = m_CC.isGrounded;
        

        if (m_IsGroundeed && m_VerticlaSpeed < 0)
        {
            m_JumpEffect.Stop();
            m_IsJump = false;
            m_VerticlaSpeed = 0f;
        }

        if (!m_IsGroundeed)
        {
            m_FallTimer -= Time.deltaTime;
            if (m_FallTimer < 0)
                m_Animator.SetBool("IsGroundted", false);
        }
        else
        {
            m_FallTimer = 0.15f;
            m_Animator.SetBool("IsGroundted", true);
        }

        

        m_CC.Move(m_MoveDirection * Time.fixedDeltaTime * m_CurrentSpeed);

        if (!m_IsElevator)
        {
            m_VerticlaSpeed += m_Gravity * Time.fixedDeltaTime;
            m_CC.Move(Vector3.up * m_VerticlaSpeed * Time.fixedDeltaTime);
        }
    }

    private void SetDefaultState()
    {
        m_CurrentState = ObstacleType.Nothing;
        m_SMR.material = DataController.Instance.GetMat(m_CurrentState);
    }

    public void Jump()
    {
        if (!m_IsJump)
        {
            m_JumpEffect.Play();
            m_IsJump = true;
            m_Animator.SetTrigger("Jump");
            m_VerticlaSpeed += Mathf.Sqrt(m_JumpHeight * -3.0f * m_Gravity);            
        }
    }

    public void Accelerate()
    {
        m_AccelerationEffect.Play();
        Sequence seq = DOTween.Sequence()
            .Append(DOTween.To(() => m_CurrentSpeed, x => m_CurrentSpeed = x, m_Speed * 1.5f, 0.1f).SetEase(Ease.OutFlash))
            .AppendInterval(1f)
            .Append(DOTween.To(() => m_CurrentSpeed, x => m_CurrentSpeed = x, m_Speed, 0.5f).SetEase(Ease.InFlash))
            .OnComplete(()=> 
            {
                if (m_AccelerationEffect)
                    m_AccelerationEffect.Stop();
            });
    }

    public void ChangeMoveDirection(Vector3 _newDir, float _radius)
    {
        float time = 2 * _radius * 1.25f * Mathf.PI / 4f / m_CurrentSpeed;
        Sequence seq = DOTween.Sequence()
            .Append(DOTween.To(() => m_MoveDirection, x => m_MoveDirection = x, _newDir, time)
            .SetEase(Ease.Linear));
    }

    public void Death()
    {
        for (int i = 0; i < m_HummanoidParts.Length; i++)
        {
            m_HummanoidParts[i].transform.parent = transform.parent;
            m_HummanoidParts[i].Activate(transform.position, (m_CC.velocity + m_MoveDirection).normalized, m_SMR.material);
        }

        Destroy(gameObject);
    }

    public void SetHologram()
    {
        m_CurrentState = ObstacleType.Hologram;
        m_SMR.material = DataController.Instance.GetMat(m_CurrentState);

        m_StateTimer?.Kill();
        m_StateTimer = DOTween.Sequence()
            .AppendInterval(2f)
            .OnComplete(() => { SetDefaultState(); });
    }

    public void SetHeaviness()
    {
        m_CurrentState = ObstacleType.Heaviness;
        m_SMR.material = DataController.Instance.GetMat(m_CurrentState);

        m_StateTimer?.Kill();
        m_StateTimer = DOTween.Sequence()
            .AppendInterval(2f)
            .OnComplete(() => { SetDefaultState(); });
    }

    public void SetBlowerMove(Vector3 _direction)
    {
        m_CC.Move(_direction * 15f * Time.deltaTime);
    }

    public void SetElevatorState(bool _value)
    {
        m_IsElevator = _value;
        m_MagnetObject.SetActive(m_IsElevator);

        if (!_value)
        {
            m_Animator.SetTrigger("Run");
            m_SMR.material = DataController.Instance.GetMat(ObstacleType.Nothing);
        }
        else
        {
            m_Animator.SetTrigger("Hanging");
            m_SMR.material = DataController.Instance.GetMat(ObstacleType.NumberOfTypes);
            m_StateTimer?.Kill();
        }
    }
}
