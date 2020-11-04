using System.Collections;
using System.Collections.Generic;
//using UnityEditor.MemoryProfiler;
using UnityEngine;

public abstract class AIBase : MonoBehaviour {
	public Color Color = Color.gray;

	[SerializeField] protected float _health = 4f;
	[SerializeField] protected float _spd = 2f;
	[SerializeField] protected float _detectRadius = 2f;
	[SerializeField] protected float _patrolDistance = 2f;

	protected bool _isAlert = false;

	protected Vector2 _initPos;

	protected GameObject _player;

	protected Vector2 _vel;

	protected AudioManager _audio;

	protected bool _alertPlayed = false;

	protected SpriteRenderer _render;

	protected bool _inAir = true;

    // Start is called before the first frame update
    protected void Start() {
		_initPos = transform.position;
		_player = GameObject.FindGameObjectWithTag("Player");
		_vel = new Vector2(_spd, 0);
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		_render = GetComponent<SpriteRenderer>();
		_render.color = Color;
	}

    // Update is called once per frame
    protected void Update() {
		if (_isAlert)
			DoAlert();
		else
			DoPatrol();

		_vel.y = 9.8f * Time.deltaTime;

		transform.Translate(_vel * Time.deltaTime);
    }

	protected void DoPatrol() {
		// detect player
		if (Vector3.SqrMagnitude(_player.transform.position - transform.position) <= _detectRadius * _detectRadius) {
			_isAlert = true;
			return;
		}
		// check to turn around if moving right or left
		// moving right
		if ((_vel.x > 0) && (transform.position.x > _initPos.x + _patrolDistance)) {
			_vel = new Vector2(-_spd, 0);
		}
		else if ((_vel.x < 0) && (transform.position.x < _initPos.x - _patrolDistance)) {
			_vel = new Vector2(_spd, 0);
		}
	}

	protected virtual void DoAlert() { }

	// damage and sounds
	protected void OnCollisionEnter2D(Collision2D collision) {
		if (collision.transform.tag == "Bullet") {
			_audio.PlayBulletImpacts();
			_health--;
			if (_health <= 0) {
				_audio.PlayBodySplat();
				Destroy(gameObject);
			}
		}
		else if (collision.transform.tag == "Crate") {
			_vel = new Vector2(-_vel.x, 0);
		}

		if (collision.contacts[0].normal.y > 0) {
			_inAir = false;
			_vel.y = 0;
		}
		else
			_inAir = true;
	}
}
