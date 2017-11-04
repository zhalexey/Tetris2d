using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelMenuController : MonoBehaviour
{
	public GameObject musicOnBtn;
	public GameObject musicOffBtn;
	public GameObject soundOnBtn;
	public GameObject soundOffBtn;

	public GameObject canvas;

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
		canvas.SetActive (true);
	}

	public void DeactivateMenu ()
	{
		canvas.SetActive (false);
		UpdateAudioControllers ();
	}

	public void OnBackToMainMenuClick ()
	{
		DeactivateMenu ();
		ScriptManager.GameController.ContinueGameFlow ();
		SceneManager.LoadScene ("MainMenu");
	}

	public void SwitchMusicOff ()
	{
		SetMusicBtnOff ();
		ScriptManager.SoundController.MusicOff ();
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
