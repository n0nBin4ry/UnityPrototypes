using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	// one source dedicated to music/ambeiance
	public AudioSource Ambiance;
	// another source dedicated to sfx
	public AudioSource SFX;
	
	[SerializeField] private AudioClip _fireLightAudio;
	[SerializeField] private AudioClip _ExplosionAudio;

	void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

	// have  special fire calls so that fire prefabs don't take up so much memory
	public void LightFire() {
		// TODO: do a more elaborate fire sound system if needed
		SFX.PlayOneShot(_fireLightAudio);
	}

	public void Explode() {
		SFX.PlayOneShot(_ExplosionAudio);
	}
}
