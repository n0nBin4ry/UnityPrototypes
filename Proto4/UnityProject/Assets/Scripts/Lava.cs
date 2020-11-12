using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {

	public float OscillationAmplitude = .5f;
	public float MinOscillationSpeed = .5f;
	public float MaxOscillationSpeed = 1f;
	[SerializeField] private Transform m_screenAnchor;
	private float m_oscillationSpeed;
	private bool m_moveForward = true;

	[HideInInspector] public float m_HorizontalMovementSpeed = 0f;
	[SerializeField] Material m_FlameMaterial;
	[SerializeField] Material m_FlameMaterialSTATIC;
	Color m_OriginalFlameTintColor;
	Color m_OriginalFlameEdgeColor;
	[SerializeField] Color m_SlowdownFlameTintColor;
	[SerializeField] Color m_SlowdownFlameEdgeColor;
	[SerializeField] float m_SlowdownEffectDuration = 1f;
	float m_CurrDuration = 0f;
	bool m_ShowEffect = false;

	private bool m_HasCameraStopped = false;

	private void Start() {
		m_oscillationSpeed = MinOscillationSpeed;
		m_CurrDuration = 0f;
		m_ShowEffect = false;
		// NEEDED for edge case of restarting game when slowdown effect going on
		m_FlameMaterial.SetColor("_Tint", m_FlameMaterialSTATIC.GetColor("_Tint"));
		m_FlameMaterial.SetColor("_EdgeColor", m_FlameMaterialSTATIC.GetColor("_EdgeColor"));
		m_OriginalFlameTintColor = m_FlameMaterial.GetColor("_Tint");
		m_OriginalFlameEdgeColor = m_FlameMaterial.GetColor("_EdgeColor");
	}

	// Update is called once per frame
	void Update() {
		if(m_ShowEffect)
        {
			m_CurrDuration += Time.deltaTime;
			if(m_CurrDuration > m_SlowdownEffectDuration)
            {
				m_CurrDuration = m_SlowdownEffectDuration;
				m_ShowEffect = false;
            }
        }
		else
        {
			m_CurrDuration -= Time.deltaTime;
			if(m_CurrDuration < 0f)
            {
				m_CurrDuration = 0f;
            }
        }

		Color color = Color.Lerp(m_OriginalFlameTintColor, m_SlowdownFlameEdgeColor, m_CurrDuration / m_SlowdownEffectDuration);
		m_FlameMaterial.SetColor("_Tint", color);
		color = Color.Lerp(m_OriginalFlameEdgeColor, m_SlowdownFlameTintColor, m_CurrDuration / m_SlowdownEffectDuration);
		m_FlameMaterial.SetColor("_EdgeColor", color);

		if (m_HasCameraStopped)
			return;

		// move lava back and forth a bit at a randomized rate for more life-like feeling
		if (m_moveForward)
			transform.Translate(Vector3.right * m_oscillationSpeed * Time.deltaTime);
		else
			transform.Translate(Vector3.right * -m_oscillationSpeed * Time.deltaTime);

		float anchorDistSqr = Vector3.SqrMagnitude(transform.position - m_screenAnchor.position);
		if (anchorDistSqr > OscillationAmplitude * OscillationAmplitude) {
			m_moveForward = !m_moveForward;
			m_oscillationSpeed = Random.Range(MinOscillationSpeed, MaxOscillationSpeed);
		}

		transform.Translate(m_HorizontalMovementSpeed * Time.deltaTime, 0, 0);

		bool isLavaHorizontalPositionPastCamera = transform.position.x >= Camera.main.transform.position.x;

		if (isLavaHorizontalPositionPastCamera)
			m_HasCameraStopped = true;

		
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        
		if (collision.tag == "Enemy") {
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy.Burn();
            //Destroy(collision.gameObject);
        } else if (collision.gameObject.tag == "Player") {
			collision.gameObject.GetComponent<PlayerController>().Burn();
		}
    }

	public void EnableSlowdownEffect()
    {
		m_ShowEffect = true;
		m_CurrDuration = 0f;
    }
}
