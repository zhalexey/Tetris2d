using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLevelMenuCanvasController : BaseCanvasController
{
	public void OnNext() {
		gameObject.SetActive (false);
		ScriptManager.LevelMenuController.OnNext ();
	}
}
