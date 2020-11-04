using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingTrapController : MonoBehaviour {
	public float swing_speed = 50f;

	public float swing_max = 180f;

	public float sound_distance = 90f;

	AudioSource[] swingSFXs;

	float rotation_distance = 0f;

	bool swingRight = true;
	bool soundPlayed = false;

    // Start is called before the first frame update
    void Start() {
		swingSFXs = GetComponentsInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
		// swing
		if (swingRight)
			transform.Rotate(Vector3.up, -swing_speed * Time.deltaTime);
		else
			transform.Rotate(Vector3.up, swing_speed * Time.deltaTime);
		rotation_distance += swing_speed * Time.deltaTime;
		// turn around
		if (rotation_distance >= swing_max) {
			swingRight = !swingRight;
			soundPlayed = false;
			rotation_distance = 0f;
		}
		// play sound at lower section
		if (!soundPlayed && (rotation_distance >= sound_distance)) {
			foreach (var sound in swingSFXs)
				sound.Play();
			soundPlayed = true;
		}
    }
}
