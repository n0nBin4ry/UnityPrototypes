using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class CrateController : MonoBehaviour
{
	private AudioManager _audio;
	private Camera _cam;
	private SpriteRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
		_cam = Camera.main;
		_audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		_renderer = GetComponent<SpriteRenderer>();
	}

	// collisions for sounds (and breaking maybe in future?)
	private void OnCollisionEnter2D(Collision2D collision) {
		// play bullet sound if bullet
		if (collision.transform.tag == "Bullet") {
			_audio.PlayBulletImpacts();
		}
		// play sound of getting bumped into always (if on screen)
		if (_renderer.isVisible) {
			_audio.PlayCrateImpacts();
		}
	}
}
