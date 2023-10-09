using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private Vector2 m_DefaultResolution = new Vector2(720, 1280);
    [Range(0f, 1f)] [SerializeField] private float m_WidthOrHeight = 0;

    private Camera m_Camera;
    private float m_InitialSize;
    private float m_TargetAspect;

    private float m_InitialFov;
    private float m_HorizontalFov = 120f;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_InitialSize = m_Camera.orthographicSize;
        m_TargetAspect = m_DefaultResolution.x / m_DefaultResolution.y;
        m_InitialFov = m_Camera.fieldOfView;
        m_HorizontalFov = CalculateVerticalFov(m_InitialFov, 1 / m_TargetAspect);
        SetSize();
    }

    private void SetSize()
    {
        if (m_Camera.orthographic)
        {
            float constantWidthSize = m_InitialSize * (m_TargetAspect / m_Camera.aspect);
            m_Camera.orthographicSize = Mathf.Lerp(constantWidthSize, m_InitialSize, m_WidthOrHeight);
        }
        else
        {
            float constantWidthFov = CalculateVerticalFov(m_HorizontalFov, m_Camera.aspect);
            m_Camera.fieldOfView = Mathf.Lerp(constantWidthFov, m_InitialFov, m_WidthOrHeight);
        }
    }

    //private void Update()
    //{
    //    if (m_Camera.orthographic)
    //    {
    //        float constantWidthSize = m_InitialSize * (m_TargetAspect / m_Camera.aspect);
    //        m_Camera.orthographicSize = Mathf.Lerp(constantWidthSize, m_InitialSize, m_WidthOrHeight);
    //    }
    //    else
    //    {
    //        float constantWidthFov = CalculateVerticalFov(m_HorizontalFov, m_Camera.aspect);
    //        m_Camera.fieldOfView = Mathf.Lerp(constantWidthFov, m_InitialFov, m_WidthOrHeight);
    //    }
    //}

    private float CalculateVerticalFov(float _hFovInDeg, float _aspectRatio)
    {
        float hFovInRads = _hFovInDeg * Mathf.Deg2Rad;
        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / _aspectRatio);
        return vFovInRads * Mathf.Rad2Deg;
    }
}
