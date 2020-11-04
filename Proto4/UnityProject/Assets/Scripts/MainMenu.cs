using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1); //Load next scene; this will be scene 0
    }

    public void QuitGame()
    {
        Application.Quit();
    }

	public void GoToMainMenu() {
		SceneManager.LoadScene(0);
	}
}
