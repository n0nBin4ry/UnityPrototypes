using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMove : MonoBehaviour
{
    [SerializeField] float m_Speed = 5f;
    [SerializeField] float m_Length = 1f;
    Vector3 m_OriginalPosition;
    Vector3 m_LeftPosition;
    Vector3 m_RightPosition;
    float m_CurrTime = 0f;
    float m_RandomSpeedModifier;
    float m_RandomTime;

    // Start is called before the first frame update
    void Start()
    {
        m_OriginalPosition = transform.localPosition;
        m_LeftPosition = m_OriginalPosition + Vector3.left * m_Length / 2.0f + Vector3.down * m_Length / 2.0f;
        m_RightPosition = m_OriginalPosition + Vector3.right * m_Length / 2.0f + Vector3.up * m_Length / 2.0f;
        m_RandomTime = Random.Range(1f, 5f);
        m_RandomSpeedModifier = Random.Range(1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        m_RandomTime -= Time.deltaTime;
        if (m_RandomTime < 0f)
        {
            m_RandomTime = Random.Range(1f, 5f);
            m_RandomSpeedModifier = Random.Range(1f, 2f);
        }

        m_CurrTime += Time.deltaTime * m_Speed * m_RandomSpeedModifier;
        if(m_CurrTime > (Mathf.PI * 2))
        {
            m_CurrTime = 0f;
        }

        float t = (Mathf.Sin(m_CurrTime * Mathf.Deg2Rad) / 2.0f) + 0.5f; //get [-1, 1] range
        transform.localPosition = Vector3.Lerp(m_LeftPosition, m_RightPosition, t);
    }
}
