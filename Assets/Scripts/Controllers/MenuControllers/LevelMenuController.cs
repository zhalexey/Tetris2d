﻿using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
	private const string TIME_OVER = "Time is over";
	private const string PLACE_OVER = "Place is over";

	public GameObject musicOnBtn;
	public GameObject musicOffBtn;
	public GameObject soundOnBtn;
	public GameObject soundOffBtn;

	public GameObject menuCanvas;
	public GameObject gameOverMenuCanvas;
	public GameObject nextLevelMenuCanvas;
	public Text menuHeaderText;


	void Start() {
		UpdateAudioControllers ();
	}

	void UpdateAudioControllers ()
	{
		if (SoundController.isMusicOn) {
			SetMusicBtnOn ();
		}
		else {
			SetMusicBtnOff ();
		}
		if (SoundController.isSoundOn) {
			SetSoundBtnOn ();
		}
		else {
			SetSoundBtnOff ();
		}
	}

	public void ActivateMenu() {
		menuCanvas.SetActive (true);
	}

	public void DeactivateMenu ()
	{
		menuCanvas.SetActive (false);
		UpdateAudioControllers ();
	}

	public void ActivateGameOverMenu() {
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		menuHeaderText.text = PLACE_OVER;
		gameOverMenuCanvas.SetActive (true);
	}

	public void ActivateTimeOutMenu() {
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		menuHeaderText.text = TIME_OVER;
		gameOverMenuCanvas.SetActive (true);
	}

	public void ActivateNextLevelMenu() {
		nextLevelMenuCanvas.SetActive (true);
	}

	public void DeactivateNextLevelMenu() {
		ScriptManager.SoundController.PauseMenuTheme ();
		nextLevelMenuCanvas.SetActive (false);
	}

	public void DeactivateGameOverMenu ()
	{
		ScriptManager.SoundController.PauseMenuTheme ();
		gameOverMenuCanvas.SetActive (false);
	}

	public void OnBackToMainMenuClick ()
	{
		DeactivateMenu ();
		DeactivateGameOverMenu ();
		DeactivateNextLevelMenu ();
		ScriptManager.GameController.ContinueTimeFlow ();
		SceneManager.LoadScene ("MainMenu");
	}

	public void OnRetry() {
		ScriptManager.GameController.ContinueTimeFlow ();
		DeactivateGameOverMenu ();
		SceneManager.LoadScene ("Level1");
	}

	//---------------------------------- music / sound ------------------------------

	void SetMusicBtnOff ()
	{
		musicOffBtn.SetActive (true);
		musicOnBtn.SetActive (false);
	}

	void SetMusicBtnOn ()
	{
		musicOffBtn.SetActive (false);
		musicOnBtn.SetActive (true);
	}

	public void SwitchMusicOff ()
	{
		SetMusicBtnOff ();
		ScriptManager.SoundController.MusicOff ();
	}

	public void SwitchMusicOn ()
	{
		SetMusicBtnOn ();
		ScriptManager.SoundController.MusicOn ();
	}

	public void SwitchSoundOff ()
	{
		SetSoundBtnOff ();
		ScriptManager.SoundController.SoundOff ();
	}

	public void SwitchSoundOn ()
	{
		SetSoundBtnOn ();
		ScriptManager.SoundController.SoundOn ();
	}

	void SetSoundBtnOff ()
	{
		soundOffBtn.SetActive (true);
		soundOnBtn.SetActive (false);
	}

	void SetSoundBtnOn ()
	{
		soundOffBtn.SetActive (false);
		soundOnBtn.SetActive (true);
	}
}
