using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFlip : MonoBehaviour {
	public float FlipTime = 0.2f;
	private float _flipTimer = 0f;
	
    void Update() {
		if (_flipTimer > FlipTime) {
			_flipTimer = 0;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.y);
		}
		else
			_flipTimer += Time.deltaTime;
	}
}
