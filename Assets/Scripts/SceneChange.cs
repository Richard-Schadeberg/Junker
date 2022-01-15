using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void GoToScene(string scene) {
		SceneManager.LoadScene(scene);
	}
	public void QuitGame() {
		Application.Quit();
	}
}
