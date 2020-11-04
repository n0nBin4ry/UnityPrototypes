using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum SetFootsteps_Types {
		Cave,
		Grass,
		Water
	}

public class SetFootsteps : MonoBehaviour {
	public AudioClip[] caveSteps;

	public AudioClip[] grassSteps;

	public AudioClip[] waterSteps;

	FirstPersonController controller;
	AudioReverbFilter reverb;
	public SetFootsteps_Types type = SetFootsteps_Types.Cave;

	private void Start() {
		controller = GetComponent<FirstPersonController>();
		reverb = GetComponent<AudioReverbFilter>();
		setCaveSteps();
	}

	public void setCaveSteps() {
		controller.m_FootstepSounds = caveSteps;
		reverb.reverbPreset = AudioReverbPreset.Cave;
		type = SetFootsteps_Types.Cave;
	}

	public void setGrassSteps() {
		controller.m_FootstepSounds = grassSteps;
		reverb.reverbPreset = AudioReverbPreset.Forest;
		type = SetFootsteps_Types.Grass;
	}

	public void setWaterSteps() {
		controller.m_FootstepSounds = waterSteps;
		reverb.reverbPreset = AudioReverbPreset.Forest;
		type = SetFootsteps_Types.Water;
	}
}
