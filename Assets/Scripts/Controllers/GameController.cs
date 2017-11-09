﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{

	private enum State
	{
		Started,
		Finished
	}

	private const int TIME_OUT = 5 * 60;
	private const int COINS_NUMBER = 17;

	public GameObject timeScale;

	private State state;
	private float currentTime;
	private bool isGamePaused;


	void Start ()
	{
		currentTime = Time.time;
		state = State.Started;
		isGamePaused = false;

		Respawn ();
	}


	public void Respawn ()
	{
		if (State.Started == state) {
			bool isRespawnAllowed = ScriptManager.BoardController.Respawn ();
			if (!isRespawnAllowed) {
				state = State.Finished;
				ScriptManager.LevelMenuController.ActivateGameOverMenu ();
			}
		}
	}


	void Update() {
		if (State.Started == state) {

			if (!ValidateTimeOut ()) {
				state = State.Finished;
				StopTimeFlow ();
				ScriptManager.LevelMenuController.ActivateTimeOutMenu ();
				return;
			}

			if (ValidateCoinsBorrowed ()) {
				state = State.Finished;
				StopTimeFlow ();
				ScriptManager.LevelMenuController.ActivateNextLevelMenu ();
			}

		}
	}

	private bool ValidateCoinsBorrowed() {
		return ScriptManager.BoardController.ValidateCoinsBorrowed(COINS_NUMBER);
	}

	private bool ValidateTimeOut ()
	{
		Image timeScaleImage = timeScale.GetComponent<Image> ();
		float deltaTime = Time.time - currentTime;
		timeScaleImage.fillAmount = deltaTime / TIME_OUT;
		if (deltaTime >= TIME_OUT) {
			return false;
		}
		return true;
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
		ContinueTimeFlow ();
		ScriptManager.SoundController.PauseMenuTheme ();
		ScriptManager.SoundController.PlayGameTheme ();
		ScriptManager.LevelMenuController.DeactivateMenu ();
	}

	void StopTimeFlow ()
	{
		Time.timeScale = 0;
		SetPause (true);
		isGamePaused = true;
	}

	private void Pause ()
	{
		StopTimeFlow ();
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		ScriptManager.LevelMenuController.ActivateMenu ();
	}

	public void ContinueTimeFlow () {
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
