using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelConfigController : MonoBehaviour {

	public Sprite backgroundImage;
	public List<GameObject> levelFigures;
	public bool isTestMode;

	private Image timeScaleImg;
	private Image treasureBoxImg;
	private int coinsToCollect;

	public int GetCoinsToCollect() {
		return coinsToCollect;
	}

	public Image GetTimeScaleImg() {
		return timeScaleImg;
	}

	public Image GetTreasureBoxImg() {
		return treasureBoxImg;
	}

	public GameObject GetTreasureBox() {
		return treasureBoxImg.gameObject;
	}

	public bool IsSimpleGame() {
		return levelFigures == null || levelFigures.Count == 0;
	}

	void Start() {
		ScriptManager.GameController.GetInstanceID();

		GameObject camera = ScriptManager.Camera;
		GameObject backgroundCanvas = ScriptManager.BackgroundCanvas;
		backgroundCanvas.GetComponentInChildren<Image>().sprite = backgroundImage;

		backgroundCanvas.transform.SetParent(camera.transform, false);
		ScriptManager.TimeScaleCanvas.transform.SetParent (camera.transform, false);
		ScriptManager.TreasureBoxCanvas.transform.SetParent (camera.transform, false);

		timeScaleImg = GetActiveImage (ScriptManager.TimeScaleCanvas);
		treasureBoxImg = GetActiveImage (ScriptManager.TreasureBoxCanvas);

		coinsToCollect = CountCoinsToCollect ();
	}

	private int CountCoinsToCollect() {
		int counter = 0;

		foreach (GameObject figure in levelFigures) {
			Transform[] childs = figure.GetComponentsInChildren<Transform> ();
			foreach (Transform child in childs) {
				if (BoardController.COIN_TAG == child.gameObject.tag) {
					counter++;
				}
			}
		}

		return counter;
	}

	private Image GetActiveImage(GameObject canvas) {
		Image[] images = canvas.GetComponentsInChildren<Image> ();
		foreach (Image img in images) {
			if (Image.Type.Filled == img.type) {
				return img;
			}
		}
		return null;
	}


}
