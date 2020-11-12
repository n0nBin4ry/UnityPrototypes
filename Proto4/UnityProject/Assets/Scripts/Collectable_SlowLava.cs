using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable_SlowLava : Collectable {
	[Range(0f, 1f)]
	public float SlowDownPercent = 0.2f;

	protected override bool GetCollected(Collider2D collision) {
		var cam = Camera.main.GetComponent<CameraController>();
		if (cam)
			cam.SlowDown(SlowDownPercent);
		return true;
	}
}
