using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	//bool doAccelerate = false;
	[HideInInspector] public bool playerDead = false;
	[SerializeField] private float m_velocity = 1f;
	[SerializeField] private float m_acceleration = 0.2f;

	public GameObject canvas;
	public GameObject[] buttons;
	public RectTransform timeTextTrans;

	public Vector2 targetTextPos;

	public float textSpeed = .5f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
		if (!playerDead) {
			transform.Translate(m_velocity * Time.deltaTime, 0, 0);
			m_velocity += m_acceleration * Time.deltaTime;
		}
		else {
			var lava = GetComponentInChildren<Lava>();
			if (lava == null) return;
			lava.transform.parent = null;
			lava.m_velocity = Mathf.Max(m_velocity, 10f);
		}
	}

	// shows death screen
	public void showFinalScore() {
		// TODO
		canvas.GetComponent<UI>().StopTimer();
		StartCoroutine("CoShowFinalScore");
		Debug.Log("Final score shown");
	}

	bool showingFinalScore = false;
	IEnumerator CoShowFinalScore() {
		if (showingFinalScore) { yield break; }
		showingFinalScore = true;
		
		while (true) {
			timeTextTrans.position = new Vector2(Mathf.Lerp(timeTextTrans.position.x, targetTextPos.x, textSpeed * Time.deltaTime),
												Mathf.Lerp(timeTextTrans.position.y, targetTextPos.y, textSpeed * Time.deltaTime));

			if (Vector3.SqrMagnitude(timeTextTrans.position - new Vector3(targetTextPos.x, targetTextPos.y)) < .1){
				break;
			}

			yield return null;
		}

		foreach (var butt in buttons)
			butt.SetActive(true);

		yield break;
	}
}
