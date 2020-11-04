using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossController : MonoBehaviour {
	public float Speed = 5f;
	//public float Accel = 20f;

	public Color ActivationColor = Color.red;

	private SpriteRenderer _sprite;
	private Rigidbody2D _rb;
	private Vector2 _dir;

	public AudioClip AttackSound;
	public AudioClip CrashSound;
	private AudioManager _audio;

    // Start is called before the first frame update
    void Start() {
		_sprite = GetComponent<SpriteRenderer>();
		_rb = GetComponent<Rigidbody2D>();
		_rb.velocity = Vector2.zero;
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
	}

    // Update is called once per frame
    void Update() {
        
    }

	// player crushed by boss
	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.transform.tag == "Player") {
			collision.gameObject.GetComponent<PlayerController>().Die();
			_rb.velocity = Speed * _dir;
		}
		// crashed into wall
		else {
			_audio.SFX.PlayOneShot(CrashSound);

			if (Mathf.Abs(_dir.y) > Mathf.Abs(_dir.x))
				Camera.main.GetComponent<VerticalShake>().DoShake();
			else
				Camera.main.GetComponent<HorizontalShake>().DoShake();
			_rb.velocity = Vector2.zero;
		}
	}

	// player caught in trigger
	private void OnTriggerEnter2D(Collider2D collision) {
		if (_rb.velocity == Vector2.zero && collision.tag == "Player") {
			_sprite.color = ActivationColor;
			_dir = collision.transform.position - transform.position;
			if (Mathf.Abs(_dir.y) > Mathf.Abs(_dir.x))
				_dir = Vector2.down;
			else
				_dir = Vector2.left;
			_rb.velocity = Speed * _dir;
			_audio.SFX.PlayOneShot(AttackSound);
		}
	}
}
