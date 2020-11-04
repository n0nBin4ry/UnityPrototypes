using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.MemoryProfiler;
using UnityEngine;

public class Flammable : MonoBehaviour {
	public float IgniteTime = .5f;
	public float BurnoutTime = 1f;

	public GameObject FirePrefab;

	public GameObject NewObject;

	protected float _timer = 0f;

	protected FireController _fire = null;

    // Update is called once per frame
    void Update() {
		if (_fire == null)
			return;
		_timer += Time.deltaTime;
		if (_timer > BurnoutTime)
			BurnOut();
    }

	private void OnTriggerEnter2D(Collider2D collision) {
		if (_fire == null && collision.tag == "Fire" 
		&& collision.GetComponent<FireController>().PlayersFire 
		&& Vector3.SqrMagnitude(collision.transform.position - transform.position) < 0.2 * 0.2) {
			_fire = GameObject.Instantiate(FirePrefab, transform.position, Quaternion.identity).GetComponent<FireController>();
			_fire.GetComponent<SpriteRenderer>().enabled = false;
			_timer = 0f;
		}
	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (_fire == null && collision.tag == "Fire" && !collision.GetComponent<FireController>().PlayersFire) {
			_timer += Time.deltaTime;
			if (_timer > IgniteTime) {
				_fire = GameObject.Instantiate(FirePrefab, transform.position, Quaternion.identity).GetComponent<FireController>();
				_timer = 0f;
			}
		}
	}

	protected virtual void BurnOut() {
		if (NewObject != null)
			GameObject.Instantiate(NewObject, transform.position, Quaternion.identity);
		// destroy invisible fire left by player ignition
		if (!_fire.GetComponent<SpriteRenderer>().enabled)
			Destroy(_fire.gameObject);
		Destroy(gameObject);
	}
}
