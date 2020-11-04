using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGrave : MonoBehaviour {
	public GameObject Boss;

	public AudioClip DeathSound;

	private bool _spawned = false;

    // Start is called before the first frame update
    void Start() {
		transform.parent = null;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
		if (!_spawned && Boss == null) {
			GetComponent<SpriteRenderer>().enabled = true;
			GetComponent<BoxCollider2D>().enabled = true;
			GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().SFX.PlayOneShot(DeathSound);
			_spawned = true;
		}
    }
}
