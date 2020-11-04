using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecretWarp : MonoBehaviour {

	public AudioClip Audio;

	private void Start() {
		GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().GetComponent<AudioManager>().SFX.PlayOneShot(Audio);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Player") {
			Camera.main.GetComponent<LevelLoader>().FadeLoadScene("SecretRoom");
		}
	}
}
