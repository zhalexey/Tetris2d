using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuCanvasController : BaseCanvasController
{

	public GameObject musicOnBtn;
	public GameObject musicOffBtn;
	public GameObject soundOnBtn;
	public GameObject soundOffBtn;


	void Start ()
	{
		UpdateAudioControllers ();
	}

	void UpdateAudioControllers ()
	{
		if (SoundController.isMusicOn) {
			SetMusicBtnOn ();
		} else {
			SetMusicBtnOff ();
		}

		if (SoundController.isSoundOn) {
			SetSoundBtnOn ();
		} else {
			SetSoundBtnOff ();
		}
	}

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


	public void SwitchMusicOn ()
	{
		SetMusicBtnOn ();
		ScriptManager.SoundController.MusicOn ();
	}

	public void SwitchMusicOff ()
	{
		SetMusicBtnOff ();
		ScriptManager.SoundController.MusicOff ();
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

	public void OnContinue ()
	{
		gameObject.SetActive (false);
		ScriptManager.LevelMenuController.OnContinue ();
	}
}
