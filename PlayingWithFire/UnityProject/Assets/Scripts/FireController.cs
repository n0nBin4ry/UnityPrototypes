using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireController : MonoBehaviour {
	// set if it belongs to the player or not
	[HideInInspector] public bool PlayersFire = false;

	public float LifeTime = 3f;
	private float _lifeTimer = 0f;

	public float AnimationTime = 0.2f;
	private float _flipTimer = 0f;

	private SpriteRenderer _renderer;

	private bool _lit = false;

	// Start is called before the first frame update
	void Start() {
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable() {
		_lifeTimer = 0f;
		_flipTimer = 0f;
	}

	private void OnBecameVisible() {
		if (!_lit && !PlayersFire && transform.parent == null) {
			GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().LightFire();
			_lit = true;
		}
	}

	private void OnDestroy() {
//		GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().ExtinguishFire();
	}

	// Update is called once per frame
	void Update() {
		if (_lifeTimer > LifeTime) {
			if (PlayersFire)
				gameObject.SetActive(false);
			else
				Destroy(gameObject);
		}
		else
			_lifeTimer += Time.deltaTime;

		if (_flipTimer > AnimationTime) {
			_flipTimer = 0;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.y);
		}
		else
			_flipTimer += Time.deltaTime;
	}
}
