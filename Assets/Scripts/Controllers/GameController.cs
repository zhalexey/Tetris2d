using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

	private enum State
	{
		Started,
		Finished
	}

	private State state;

	private int count = 100;

	private bool isGamePaused;


	void Start ()
	{
		state = State.Started;
		isGamePaused = false;

		Respawn ();
	}


	public void Respawn ()
	{
		if (count-- == 0) {
			state = State.Finished;
		}
			

		if (State.Started == state) {
			bool isRespawnAllowed = ScriptManager.BoardController.Respawn ();
			if (!isRespawnAllowed) {
				state = State.Finished;
			}
		}

	}


	private void SetPause (bool value)
	{
		Object[] gameObjects = GameObject.FindObjectsOfType (typeof(BaseGameObjectController));
		foreach (Object obj in gameObjects) {
			BaseGameObjectController gObj = (BaseGameObjectController)obj;
			gObj.isPause = value;
		}
	}

	private void Resume ()
	{
		ContinueGameFlow ();
		ScriptManager.SoundController.PauseMenuTheme ();
		ScriptManager.SoundController.PlayGameTheme ();
		ScriptManager.LevelMenuController.DeactivateMenu ();
	}

	private void Pause ()
	{
		Time.timeScale = 0;
		SetPause (true);
		isGamePaused = true;
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		ScriptManager.LevelMenuController.ActivateMenu ();
	}

	public void ContinueGameFlow () {
		Time.timeScale = 1;
		SetPause (false);
		isGamePaused = false;
	}

	public void PauseResume ()
	{
		if (!isGamePaused) {
			Pause ();
		} else
			Resume ();
	}


}
