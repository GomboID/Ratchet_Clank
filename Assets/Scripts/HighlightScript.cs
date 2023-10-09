using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HighlightScript : MonoBehaviour
{
    [SerializeField] private Transform m_SpriteTransform;
    private Sequence m_Animation;
    private float m_Timer = 0f;

    private void Update()
    {
        if (GameController.Instance.GetState != GameState.Game)
            return;

        int touchCount = Input.touchCount;

        if (touchCount <= 0)
        {
            m_Timer += Time.deltaTime;

            if (m_Timer >= 3.5f)
            {
                m_Timer = 0f;

                m_SpriteTransform.localPosition = new Vector3(0f, 0f, -2f);

                m_Animation = DOTween.Sequence()
                    .Append(m_SpriteTransform.DOLocalMove(new Vector3(0f, 0f, 2f), 0.5f).SetEase(Ease.Linear));
            }
        }
        else
        {
            m_Timer = 0f;
        }
    }
}
