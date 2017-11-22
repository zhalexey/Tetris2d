using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {

	void Start() {
		ScriptManager.SoundController.PlayMenuTheme ();
	}

	public void OnBack ()
	{
		SceneManager.LoadScene (GameController.START_MENU_SCENE);
	}

}
