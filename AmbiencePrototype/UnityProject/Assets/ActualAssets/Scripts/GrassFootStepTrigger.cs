using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassFootStepTrigger : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			var temp = other.GetComponent<SetFootsteps>();
			if (temp.type == SetFootsteps_Types.Grass)
				return;
			temp.setGrassSteps();
		}
	}
}
