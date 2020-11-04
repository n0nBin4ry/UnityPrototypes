using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public AudioClip[] Footsteps;
	private int _footstepIndex = 0;

	public AudioClip[] Jumps;

	public AudioClip[] Gunshots;

	public AudioClip[] BulletImpacts;

	public AudioClip[] CrateImpacts;

	public AudioClip[] BodySplats;

	public AudioClip AttackAlert;

	public AudioClip FleeAlert;

	public AudioClip CrateBreak;

	private AudioSource _player;

	private AudioReverbFilter _reverb;

    // Start is called before the first frame update
    void Start() {
		_player = GetComponent<AudioSource>();
		_reverb = GetComponent<AudioReverbFilter>();
    }

    // Update is called once per frame
    void Update() {
		// debug/testing
	
    }

	public void PlayFootstep() {
		_player.PlayOneShot(Footsteps[_footstepIndex]);
		_footstepIndex = (_footstepIndex + 1) % Footsteps.Length;
	}

	public void PlayGunshot() {
		// plays 2 random gunshot sounds in tandum to add uniqueness
		_player.PlayOneShot(Gunshots[Random.Range(0, Gunshots.Length)]);
		_player.PlayOneShot(Gunshots[Random.Range(0, Gunshots.Length)]);
	}

	public void PlayCrateImpacts() {
		_player.PlayOneShot(CrateImpacts[Random.Range(0, CrateImpacts.Length)]);
	}

	public void PlayBulletImpacts() {
		_player.PlayOneShot(BulletImpacts[Random.Range(0, BulletImpacts.Length)]);
	}

	public void PlayBodySplat() {
		// plays 2 splat sounds in tandum to add uniqueness
		_player.PlayOneShot(BodySplats[Random.Range(0, BodySplats.Length)]);
		_player.PlayOneShot(BodySplats[Random.Range(0, BodySplats.Length)]);
	}

	public void PlayJump() {
		_player.PlayOneShot(Jumps[0]);
	}

	public void PlayAttackAlert() {
		_player.PlayOneShot(AttackAlert);
	}

	public void PlayFleeAlert() {
		_player.PlayOneShot(FleeAlert);
	}

	public void PlayCrateBreak() {
		_player.PlayOneShot(CrateBreak);
	}
}
