using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class JumpTrailObj : MonoBehaviour {
	public float MaxScale = 1.85f;
	public float MinScale = 0.9f;
	public float ScaleRate = 0.83f;
	public Transform UpperBound = null;
	public Transform LowerBound = null;

	public Sprite SpeedUpSprite = null;
	public Sprite SlowDownSprite = null;
	public float SpeedUpEffectDuration = 1f;
	public float SlowDownEffectDuration = 2f;
	private Sprite m_DefaultSprite = null;
	private float m_effectTimer = 0f;
	private bool m_inEffect = false;

	private SpriteRenderer m_render;

    // Start is called before the first frame update
    void Start() {
		m_render = GetComponent<SpriteRenderer>();
		m_DefaultSprite = m_render.sprite;
		if (!LowerBound && transform.parent) {
			LowerBound = transform.parent.transform;
		}
		transform.parent = null;
		if (transform.localScale.x > MaxScale) {
			float temp = MinScale + Mathf.Repeat(transform.localScale.x, MaxScale - MinScale);
			transform.localScale = new Vector3(temp, temp);
		}
		if (transform.localScale.x < MinScale) {
			transform.localScale = new Vector3(MaxScale - transform.localScale.x, MaxScale - transform.localScale.x);
		}
    }

    // Update is called once per frame
    void Update() {
		Vector3 temp = transform.position;
		temp.x = LowerBound.position.x;
		transform.position = temp;
		if (!UpperBound || !LowerBound)
			Destroy(gameObject);
		// don't be visible if out of bounds
		if (transform.position.y < LowerBound.position.y || transform.position.y > UpperBound.position.y)
			m_render.enabled = false;
		else
			m_render.enabled = true;
		// always shrink either way though to keep visual pattern
		transform.localScale -= Vector3.one * (ScaleRate * Time.deltaTime);
		if (transform.localScale.x < MinScale) {
			transform.localScale = new Vector3(MaxScale, MaxScale);
			if (m_render.sprite != m_DefaultSprite && !m_inEffect)
				m_render.sprite = m_DefaultSprite;
		}
		// only play effects for short time
		if (m_inEffect) {
			m_effectTimer -= Time.deltaTime;
			if (m_effectTimer < 0f) {
				m_inEffect = false;
				//m_render.sprite = m_DefaultSprite;
			}
		}
    }

	public void ShowSlowDown() {
		m_inEffect = true;
		m_effectTimer = SlowDownEffectDuration;
		if (m_render)
			m_render.sprite = SlowDownSprite;
	}

	public void ShowSpeedUp() {
		m_inEffect = true;
		m_effectTimer = SpeedUpEffectDuration;
		if (m_render)
			m_render.sprite = SpeedUpSprite;
	}

	public void SetColor(UnityEngine.Color color) {
		if (!m_render)
			return;
		color.a = m_render.color.a;
		m_render.color = color;
	}
}
