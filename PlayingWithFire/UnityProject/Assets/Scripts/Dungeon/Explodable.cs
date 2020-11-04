using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : Flammable {

	protected override void BurnOut() {
		GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Explode();

		if (NewObject != null)
			GameObject.Instantiate(NewObject, transform.position, Quaternion.identity);
		// destroy invisible fire left by player ignition
		if (!_fire.GetComponent<SpriteRenderer>().enabled)
			Destroy(_fire.gameObject);
		Camera.main.GetComponent<VerticalShake>().DoShake();
		if (transform.parent != null)
			Destroy(transform.parent.gameObject);
	}
}
