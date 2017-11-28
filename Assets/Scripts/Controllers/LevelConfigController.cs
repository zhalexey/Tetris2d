using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfigController : MonoBehaviour {

	public GameObject gameManager;
	public List<GameObject> levelFigures;
	public GameObject treasureBox;
	public int coinsToCollect;
	public GameObject timeScale;
	public bool isTestMode;


	public bool IsSimpleGame() {
		return levelFigures == null || levelFigures.Count == 0;
	}

	void Awake() {
		GameObject obj = Instantiate (gameManager);
		obj.name = ScriptManager.GAME_MANAGER;
	}

}
