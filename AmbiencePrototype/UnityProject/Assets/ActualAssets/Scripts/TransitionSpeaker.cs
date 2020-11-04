using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionSpeaker : MonoBehaviour {
	public AudioClip caveSound;
	public AudioClip safeSound;

	AudioSource audio;

	bool caveSide = true;

	// Start is called before the first frame update
    void Start() {
		audio = GetComponent<AudioSource>();
    }

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			// if coming from cave side, mark
			if (other.transform.position.z < transform.position.z)
				caveSide = true;
			// if coming from safe side, mark
			else
				caveSide = false;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			var ambience = other.GetComponentInChildren<SetAmbience>();
			var other_audio = ambience.audio;
			var other_time = other_audio.time;
			var other_clip = other_audio.clip;
			var our_time = audio.time;
			// if crossing to safe side
			if (other.transform.position.z >= transform.position.z) {
				// if coming from same side we entered, change nothing
				if (!caveSide)
					return;
				// set ambient to safe sound at our time
				ambience.setSafe(our_time);

				// set us to cave
				setCave(other_time);

				caveSide = false;
				/*// else, swap ambient sounds
				other_audio.Stop();
				audio.Stop();
				
				// set our clip to ambients old clip
				audio.clip = other_clip;
				audio.time = other_time;
				audio.loop = true;
				audio.Play();

				// set ambient to safe sound at our time
				ambience.setSafe(our_time);*/
			}
			// if crossing to cave side
			else {
				if (caveSide)
					return;
				// set ambient to cave sound at our time
				ambience.setCave(our_time);

				// set us to safe
				setSafe(other_time);

				caveSide = true;
				/*// else, swap ambient sounds
				other_audio.Stop();
				audio.Stop();
				
				// set our clip to ambients old clip
				audio.clip = other_clip;
				audio.time = other_time;
				audio.loop = true;
				audio.Play();

				// set ambient to cave sound at our time
				ambience.setCave(our_time);*/
			}
		}
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
