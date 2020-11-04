using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndyBulletController : MonoBehaviour {
	public float DelayTime = 1f;
	public float WindupSpeed = 5f;
	public float WindupDist = 5f;
	public float ShootSpeed = 10f;

	private GameObject _target = null;
	private float _dist = 0f;
	private float _timer = 0f;

	private SpriteRenderer _sprite;

	private void Start() {
		_sprite = GetComponent<SpriteRenderer>();
		_sprite.enabled = false;
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update() {
		if (_target == null)
			return;
		else if (_timer < DelayTime) {
			_timer += Time.deltaTime;
			if (_timer >= DelayTime)
				_sprite.enabled = true;
		}
		else if (_dist < WindupDist) {
			var temp = Time.deltaTime * WindupSpeed;
			_dist += temp;
			transform.Translate(Vector3.up * temp);
		}
		else {
			var targPos = _target.transform.position;
			targPos.z = transform.position.z;
			transform.position = Vector3.Lerp(transform.position, targPos, ShootSpeed * Time.deltaTime);
		}
    }

	public void Init(GameObject target) {
		_target = target;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player") {
			var player = collision.GetComponent<PlayerController>();
			player.LoseText.text = "I think they meant turn off the game and play Spelunky instead.";
			player.Die();
			Camera.main.GetComponent<VerticalShake>().DoShake();
			Destroy(gameObject);
		}
	}
}
