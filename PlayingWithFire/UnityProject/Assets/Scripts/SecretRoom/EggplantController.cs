using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggplantController : MonoBehaviour {
	public GameObject Andy;

	public AudioClip ErrorSound;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Fire") {
			GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().GetComponent<AudioManager>().SFX.PlayOneShot(ErrorSound, .4f);
			Andy.GetComponent<AndyController>().ShootPlayer();
		}
	}
}
