using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class VerticalShake : MonoBehaviour {
	public Transform Anchor = null;

	public float Amplitude = 1f;

	public float ShakeSpeed = 10f;

	public float ShakeAccel = 70f;

	public float ShakeDuration = 0.8f;

	private bool _alternate = true;

	private bool _isShaking = false;

	public bool IsShaking() { return _isShaking; }

	public void DoShake() {
		StartCoroutine("CoDoShake");
	}

	IEnumerator CoDoShake() {
		if (_isShaking)
			yield break;
		_isShaking = true;

		float currSpd = 0f;
		float timer = 0f;

		UnityEngine.Vector3 origin = UnityEngine.Vector3.zero;
		if (Anchor == null)
			origin = transform.position;

		while (timer < ShakeDuration) {
			if (currSpd < ShakeSpeed)
				Mathf.Clamp(currSpd += ShakeAccel * Time.deltaTime, currSpd, ShakeSpeed);

			if (_alternate)
				transform.Translate(UnityEngine.Vector3.up * currSpd * Time.deltaTime);
			else
				transform.Translate(UnityEngine.Vector3.down * currSpd * Time.deltaTime);

			if (Anchor == null) {
				if (Mathf.Abs(transform.position.y - origin.y) > Amplitude) {
					if (_alternate && transform.position.y > origin.y)
						_alternate = !_alternate;
					else if (!_alternate && transform.position.y < origin.y)
						_alternate = !_alternate;
				}
			}
			else {
				if (Mathf.Abs(transform.position.y - Anchor.transform.position.y) > Amplitude) {
					if (_alternate && transform.position.y > Anchor.transform.position.y)
						_alternate = !_alternate;
					else if (!_alternate && transform.position.y < Anchor.transform.position.y)
						_alternate = !_alternate;
				}
			}

			timer += Time.deltaTime;
			yield return null;
		}

		float dist;
		if (Anchor == null)
			dist = Mathf.Abs(transform.position.y - origin.y);
		else
			dist = Mathf.Abs(transform.position.y - Anchor.transform.position.y);

		while (dist > 0.1f) {
			if (Anchor == null) {
				transform.position = UnityEngine.Vector3.Lerp(transform.position, origin, currSpd * Time.deltaTime);
				dist = Mathf.Abs(transform.position.y - origin.y);
			}
			else {
				var temp = Anchor.transform.position;
				temp.z = transform.position.z;
				transform.position = UnityEngine.Vector3.Lerp(transform.position, temp, currSpd * Time.deltaTime);
				dist = Mathf.Abs(transform.position.y - temp.y);
			}
			yield return null;
		}

		if (Anchor == null) {
			transform.position = origin;
		}
		else {
			var temp = Anchor.transform.position;
			temp.z = transform.position.z;
			transform.position = temp;
		}

		_alternate = true;
		_isShaking = false;
		yield break;
	}
}
