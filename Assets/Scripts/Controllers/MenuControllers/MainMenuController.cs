using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	public GameObject musicOnBtn;
	public GameObject musicOffBtn;
	public GameObject soundOnBtn;
	public GameObject soundOffBtn;


	void Start() {
		ScriptManager.SoundController.PlayMenuTheme ();

		if (SoundController.isMusicOn) {
			SwitchMusicOn ();
		} else {
			SwitchMusicOff ();
		}

		if (SoundController.isSoundOn) {
			SwitchSoundOn ();
		} else {
			SwitchSoundOff ();
		}
	}

	public void onStartClick ()
	{
		ScriptManager.SoundController.PauseMenuTheme ();
		SceneManager.LoadScene ("Level1");
	}

	public void onQuitClick ()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit ();
		#endif
	}

	public void SwitchMusicOff ()
	{
		musicOffBtn.SetActive(true);
		musicOnBtn.SetActive(false);
		ScriptManager.SoundController.MusicOff ();
	}

	public void SwitchMusicOn ()
	{
		musicOffBtn.SetActive(false);
		musicOnBtn.SetActive(true);
		ScriptManager.SoundController.MusicOn ();
	}

	public void SwitchSoundOff ()
	{
		soundOffBtn.SetActive(true);
		soundOnBtn.SetActive(false);
		ScriptManager.SoundController.SoundOff ();
	}

	public void SwitchSoundOn ()
	{
		soundOffBtn.SetActive(false);
		soundOnBtn.SetActive(true);
		ScriptManager.SoundController.SoundOn ();
	}

}
