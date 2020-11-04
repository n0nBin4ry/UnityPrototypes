using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
	float PostFadeDelay = 0.5f;
	public float FadeRate = 0.5f;
	public GameObject FadePanel;
	private CanvasRenderer _fadePanel;

	private string _sceneName = "";
	private bool _isFading = false;

	public AudioClip StartSound;

    // Start is called before the first frame update
    void Start() {
		FadePanel.SetActive(true);
		_fadePanel = FadePanel.GetComponent<CanvasRenderer>();
		_fadePanel.SetAlpha(0f);
    }

    // Update is called once per frame
    void Update() {
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			if (SceneManager.GetActiveScene().name == "TitleScreen")
				return;
			FadeLoadScene("TitleScreen");
		}
		else if (Input.GetKeyDown(KeyCode.R)) {
			if (SceneManager.GetActiveScene().name == "TitleScreen" && StartSound != null) { 
				var audio = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().SFX;
				//audio.loop = false;
				//audio.Stop();
				audio.PlayOneShot(StartSound);
			}

			FadeLoadScene("DungeonScene");
		}
    }

	public void FadeLoadScene(string sceneName) {
		_sceneName = sceneName;
		StartCoroutine("CoLoadNextRoom");
	}

	IEnumerator CoLoadNextRoom() {
		if (_isFading)
			yield break;
		_isFading = true;

		float alpha = 0f;

		while (alpha < 1f) {
			alpha = Mathf.Min(alpha + (Time.deltaTime * FadeRate), 1f);
			_fadePanel.SetAlpha(alpha);
			yield return null;
		}

		alpha = 0f;

		while (alpha < PostFadeDelay)
			alpha += Time.deltaTime;

		SceneManager.LoadScene(_sceneName);

		yield break;
	}
}
