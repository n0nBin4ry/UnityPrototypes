using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveFootStepTrigger : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			var temp = other.GetComponent<SetFootsteps>();
			if (temp.type == SetFootsteps_Types.Cave)
				return;
			temp.setCaveSteps();
		}
	}
}
