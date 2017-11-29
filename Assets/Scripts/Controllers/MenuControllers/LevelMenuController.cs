using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{

	private const string TIME_OVER = "Time is over";
	private const string PLACE_OVER = "Place is over";


	public void ActivateMenu() {
		ScriptManager.LevelMenuCanvas.SetActive (true);
	}

	public void ActivateGameOverMenu() {
		ActivateGameOverMenu (PLACE_OVER);
	}

	public void ActivateTimeOutMenu() {
		ActivateGameOverMenu (TIME_OVER);
	}

	void ActivateGameOverMenu (string text)
	{
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		var gameOverMenuCanvas = ScriptManager.GameOverMenuCanvas;
		Text textComp = ScriptManager.GameOverMenuCanvas.GetComponentInChildren<Text> () as Text;
		textComp.text = text;
		gameOverMenuCanvas.SetActive (true);
	}

	public void ActivateNextLevelMenu() {
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		ScriptManager.NextLevelMenuCanvas.SetActive (true);
	}

	public void OnContinue ()
	{
		ScriptManager.GameController.ContinueTimeFlow ();
		ScriptManager.SoundController.PauseMenuTheme ();
		ScriptManager.SoundController.PlayGameTheme ();
	}

	public void OpenMenu ()
	{
		ScriptManager.GameController.StopTimeFlow ();
		ScriptManager.SoundController.PauseGameTheme ();
		ScriptManager.SoundController.PlayMenuTheme ();
		ActivateMenu ();
	}

	public void OnBackToMainMenu ()
	{
		ScriptManager.GameController.ContinueTimeFlow ();
		SceneManager.LoadScene (GameController.START_MENU_SCENE);
	}

	public void OnNext() {
		ScriptManager.GameController.ContinueTimeFlow ();
		ScriptManager.SoundController.PauseMenuTheme ();

		if (PlayerController.HasAchievedLevel (GameController.MAX_LEVEL)) {
			PlayerController.ResetLevel ();
			SceneManager.LoadScene (GameController.CREDITS_SCENE);
			return;
		}
		PlayerController.NextLevel ();
		SceneManager.LoadScene (GameController.MAP_SCENE);
	}

	public void OnRetry() {
		ScriptManager.SoundController.PauseMenuTheme ();
		ScriptManager.GameController.ContinueTimeFlow ();
		bool isSimpleGame = ScriptManager.LevelConfigController.IsSimpleGame ();
		SceneManager.LoadScene (isSimpleGame ? GameController.SIMPLE_GAME_SCENE : GameController.GetLevelScene());
	}



}
