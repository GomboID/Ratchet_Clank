using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerScript : MonoBehaviour
{
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameController.Instance.Action_Win += Win;
        GameController.Instance.Action_Lose += Lose;
    }

    private void OnDisable()
    {
        GameController.Instance.Action_Win -= Win;
        GameController.Instance.Action_Lose -= Lose;
    }

    private void Win()
    {
        m_Animator.SetTrigger("Win");
    }

    private void Lose()
    {
        m_Animator.SetTrigger("Lose");
    }
}
