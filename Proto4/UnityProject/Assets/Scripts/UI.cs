using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    float timeAlive = 0f;
    public Text timeText;
	bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
		if (stop)
			return;
        timeAlive += Time.deltaTime;
        string timeAliveString = timeAlive.ToString();
        timeText.text = "Time alive: " + timeAlive.ToString("0.00");
    }

	public void StopTimer() {
		stop = true;
	}
}
