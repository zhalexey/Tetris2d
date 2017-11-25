using System.Collections;
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

	public static int MAX_LEVEL = 3;
	private const string LEVEL_SCENE = "Level";
	public const string CREDITS_SCENE = "Credits";
	public const string MAP_SCENE = "Map";
	public const string START_MENU_SCENE = "StartMenu";


	private const int TIME_OUT = 5 * 60;
	private const float COIN_PROGRESS_STEP = 0.005f;

	public GameObject boardManager;

	private State state;
	private float currentTime;
	private bool isGamePaused;
	private float coinsProgress;


	void Awake() {
		GameObject obj = Instantiate (boardManager);
		obj.name = ScriptManager.BOARD_MANAGER;

	}

	void Start ()
	{
		coinsProgress = 0;
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
		CountCoinsProgress ();

		Image treasureBoxImage = ScriptManager.LevelConfigController.treasureBox.GetComponent<Image> ();
		treasureBoxImage.fillAmount = coinsProgress;

		return ScriptManager.BoardController.CheckAllParticlesFinished() && coinsProgress > 1;
	}

	void CountCoinsProgress ()
	{
		int coinsCount = ScriptManager.BoardController.GetCoinsCount ();
		float coinsLevel = (float)coinsCount / (float)ScriptManager.LevelConfigController.coinsToCollect;
		if (coinsProgress < coinsLevel) {
			coinsProgress += COIN_PROGRESS_STEP;
		}
	}

	private bool ValidateTimeOut ()
	{
		Image timeScaleImage = ScriptManager.LevelConfigController.timeScale.GetComponent<Image> ();
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

	public void StopTimeFlow ()
	{
		Time.timeScale = 0;
		SetPause (true);
		isGamePaused = true;
	}

	public void ContinueTimeFlow () {
		Time.timeScale = 1;
		SetPause (false);
		isGamePaused = false;
	}

	public void PauseResume ()
	{
		if (!isGamePaused) {
			ScriptManager.LevelMenuController.OpenMenu();
		} else
			ScriptManager.LevelMenuController.OnContinue ();
	}

	public static string GetLevelScene ()
	{
		return LEVEL_SCENE + PlayerController.levelNumber;
	}


}
