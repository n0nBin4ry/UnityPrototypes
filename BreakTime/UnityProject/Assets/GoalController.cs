using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;

public class GoalController : MonoBehaviour {
	public GameObject GlowObj;

	public float GlowSpeed = 3f;

	public float Distance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		GlowObj.transform.localScale += Vector3.one * Time.deltaTime * GlowSpeed;
		if (Vector3.SqrMagnitude(GlowObj.transform.localScale) > Vector3.SqrMagnitude(transform.localScale))
			GlowObj.transform.localScale = Vector3.zero;
    }

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.tag != "Crate")
			return;
		if (Vector3.SqrMagnitude(collision.transform.position - transform.position) < Distance * Distance) {
			Debug.Log("Game Won!");
		}
	}
}
