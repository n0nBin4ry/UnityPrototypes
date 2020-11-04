using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {
	[HideInInspector] public float m_velocity = 0f;

	bool stopped = false;

    // Start is called before the first frame update
    void Start() {
		
    }

    // Update is called once per frame
    void Update() {
		if (stopped)
			return;
		transform.Translate(m_velocity * Time.deltaTime, 0, 0);
		if (transform.position.x >= Camera.main.transform.position.x) {
			stopped = true;
			//Camera.main.GetComponent<CameraController>().showFinalScore();
		}
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Enemy") {
            Destroy(collision.gameObject);
        }
		if (collision.gameObject.tag == "Player") {
			collision.gameObject.GetComponent<PlayerController>().die();
		}
    }
}
