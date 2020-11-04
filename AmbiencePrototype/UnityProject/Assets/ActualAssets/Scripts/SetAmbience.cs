using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAmbience : MonoBehaviour {
	public AudioClip caveSound;
	public AudioClip safeSound;

	[HideInInspector] public AudioSource audio;
	//AudioReverbFilter reverb;

    // Start is called before the first frame update
    void Start() {
		audio = GetComponent<AudioSource>();
		//reverb.GetComponent<AudioReverbFilter>();
		setCave(0f);
    }

	public void setCave(float time) {
		if (audio.isPlaying)
			audio.Stop();
		audio.clip = caveSound;
		audio.loop = true;
		audio.time = time;
		//reverb.enabled = true;
		audio.Play();
	}

	public void setSafe(float time) {
		if (audio.isPlaying)
			audio.Stop();
		audio.clip = safeSound;
		audio.loop = true;
		audio.time = time;
		//reverb.enabled = false;
		audio.Play();
	}
}
