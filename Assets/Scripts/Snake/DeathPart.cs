using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathPart : MonoBehaviour
{
    private Rigidbody m_RB;
    private MeshRenderer m_MR;

    public void Activate(Vector3 _expCenter, Vector3 _moveDirection, Material _mat)
    {
        m_RB = GetComponent<Rigidbody>();
        m_MR = GetComponent<MeshRenderer>();
        m_MR.material = _mat;
        gameObject.SetActive(true);

        m_RB.isKinematic = false;
        m_RB.useGravity = true;
        m_RB.AddExplosionForce(3f, _expCenter + Vector3.up * 2f, 3f, 1f, ForceMode.Impulse);
        m_RB.AddForce(_moveDirection * 1.5f, ForceMode.Impulse);
        m_RB.AddRelativeTorque(Random.onUnitSphere * Random.Range(-10f, 10f), ForceMode.Impulse);

        StartCoroutine(DestroyCoroutine());      
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(1f);
        float scale = 1f;

        while (scale > 0)
        {
            scale -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            transform.localScale = Vector3.one * scale;
        }

        Destroy(gameObject);
    }
}
