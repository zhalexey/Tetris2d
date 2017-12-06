using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelConfigController : MonoBehaviour {

	private const float LEFT_ALIGN_FILL_RATIO_1_33 = 0.9f;
	private const float LEFT_ALIGN_FILL_RATIO_1_77 = 0.6f;

	public Sprite backgroundImage;
	public List<GameObject> levelFigures;
	public bool isTestMode;

	private Image timeScaleImg;
	private Image treasureBoxImg;
	private int coinsToCollect;

	private int screenWidth;
	private Vector3 baseArtefactScale;

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
		if (treasureBoxImg != null) {
			return treasureBoxImg.gameObject;
		}
		return null;
	}

	public bool IsSimpleGame() {
		return levelFigures == null || levelFigures.Count == 0;
	}

	void Start() {
		screenWidth = Screen.width;

		ScriptManager.GameController.GetInstanceID();

		GameObject camera = ScriptManager.Camera;
		GameObject backgroundCanvas = ScriptManager.BackgroundCanvas;
		backgroundCanvas.GetComponentInChildren<Image>().sprite = backgroundImage;

		backgroundCanvas.transform.SetParent(camera.transform, false);

		if (!IsSimpleGame ()) { 
			ScriptManager.TimeScaleCanvas.transform.SetParent (camera.transform, false);
			ScriptManager.TreasureBoxCanvas.transform.SetParent (camera.transform, false);

			InitializeArtefacts ();
			AlignArtefacts ();

			timeScaleImg = GetActiveImage (ScriptManager.TimeScaleCanvas);
			treasureBoxImg = GetActiveImage (ScriptManager.TreasureBoxCanvas);
		}

		coinsToCollect = CountCoinsToCollect ();
	}


	private void SetLeftArtefactPosition(GameObject artefact) {
		SetArtefactPosition (true, artefact);
	}

	private void SetRightArtefactPosition(GameObject artefact) {
		SetArtefactPosition (false, artefact);
	}

	private void InitializeArtefacts() {
		baseArtefactScale = ScriptManager.TreasureBoxCanvas.transform.localScale;
	}

	private void AlignArtefacts() {
		SetLeftArtefactPosition (ScriptManager.TreasureBoxCanvas);
		SetRightArtefactPosition (ScriptManager.TimeScaleCanvas);
	}

	private void SetArtefactPosition (bool isLeft, GameObject artefact) {
		
		Vector2 pos = artefact.transform.position;
		Vector2 scale = artefact.transform.localScale;

		// position
		var x1 = Camera.main.ScreenToWorldPoint (new Vector3 (isLeft ? 0 : Screen.width, 0, 0)).x;
		var boardCanvas = ScriptManager.BoardController.gameObject;
		var x2 = (isLeft ? -1 : 1) * boardCanvas.GetComponent<SpriteRenderer> ().sprite.bounds.size.x * boardCanvas.transform.localScale.x / 2f;
		var avgX = (x1 + x2) / 2;
		pos.x = avgX;

		// scale

		var aspectRatio = (float)Screen.width / (float)Screen.height;
		var multiplier = (aspectRatio > 1.34f ? LEFT_ALIGN_FILL_RATIO_1_77 : LEFT_ALIGN_FILL_RATIO_1_33);
		var f = Mathf.Abs (x1 - x2) * multiplier;
		scale.x = baseArtefactScale.x * f;
		scale.y = baseArtefactScale.y * f;

		artefact.transform.localScale = scale;
		artefact.transform.position = pos;
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

	private Image GetActiveImage(GameObject parent) {
		Image[] images = parent.GetComponentsInChildren<Image> ();
		foreach (Image img in images) {
			if (Image.Type.Filled == img.type) {
				return img;
			}
		}
		return null;
	}


	public void CheckResolutionChanged() {
		if (!Screen.width.Equals(screenWidth)) {
			AlignArtefacts ();
			screenWidth = Screen.width;
		}
	}

}
