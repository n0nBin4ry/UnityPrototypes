using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float m_RotateSpeed = 5f;
    float m_RotateAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_RotateAngle = m_RotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, m_RotateAngle);
    }
}
