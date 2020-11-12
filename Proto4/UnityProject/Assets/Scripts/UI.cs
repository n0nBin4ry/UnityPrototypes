using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    float timeAlive = 0f;
    public Text timeText;
	public Text highScoretext;
	bool stop = false;
	public HighScore Highscore;
    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0f;
    }

    // Update is called once per frame
    void Update()
    {
		if (stop) {
			if (timeAlive > Highscore.HighestScore) {
				Highscore.HighestScore = timeAlive;
			}
			highScoretext.text = "Best Time: " + Highscore.HighestScore.ToString("0.00") + " seconds";
			return;
		}
        timeAlive += Time.deltaTime;
        string timeAliveString = timeAlive.ToString();
        timeText.text = "Time Alive: " + timeAlive.ToString("0.00") + " seconds";
    }

	public void StopTimer() {
		stop = true;
	}
}
