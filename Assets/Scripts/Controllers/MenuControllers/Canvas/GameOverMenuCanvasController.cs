using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenuCanvasController : BaseCanvasController
{
	public void OnRetry() {
		gameObject.SetActive (false);
		ScriptManager.LevelMenuController.OnRetry ();
	}
}
