using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseCanvasController : MonoBehaviour
{
	public void OnBackToMainMenu ()
	{
		gameObject.SetActive (false);
		ScriptManager.LevelMenuController.OnBackToMainMenu ();
	}
}
