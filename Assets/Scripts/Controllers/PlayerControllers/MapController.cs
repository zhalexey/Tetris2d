using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
	private const int defaultWidth = 1366;
	private const int defaultHeight = 768;
	private const string REGEXP_LEVEL = "level[0-9]+";
	private const string LEVEL = "point_level";

	public Sprite enabledPointImage;
	public GameObject targetAnimation;

	private List<GameObject> points;
	private static GameObject targetPoint;

	private float xMult;
	private float yMult;
	private float scaleFactor;

	void Start ()
	{
		ScriptManager.SoundController.PlayMapMusic ();
		CollectPoints ();
		UpdatePoints ();
	}

	private void CollectPoints ()
	{
		points = new List<GameObject> ();
		Component[] components = gameObject.GetComponentsInChildren<Component> ();
		foreach (Component comp in components) {
			if (comp is Transform && IsLevelObject (comp.name)) {
				points.Add (comp.gameObject);
			}
		}
	}

	private bool IsLevelObject (string name)
	{
		return Regex.IsMatch (name, REGEXP_LEVEL);
	}

	private void UpdatePoints ()
	{
		int pointNum = 1;
		foreach (GameObject point in points) {

			if (pointNum <= PlayerController.levelMaxAchieved) {
				EnablePoint (point);
				if (pointNum == PlayerController.levelNumber) {
					MarkCurrentPoint (point);
				}
			}
			pointNum++;
		}
	}

	private void EnablePoint (GameObject point)
	{
		point.GetComponentInChildren<Image> ().sprite = enabledPointImage;
	}

	private void MarkCurrentPoint (GameObject point)
	{
		targetPoint = Instantiate (targetAnimation, new Vector3 (point.transform.position.x, point.transform.position.y, 0), Quaternion.identity);
		Canvas canvas = gameObject.GetComponentInChildren<Canvas> ();
		targetPoint.transform.SetParent (canvas.transform);
	}

	public void OnContinue ()
	{
		SceneManager.LoadScene (GameController.GetLevelScene ());
	}


	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast (Input.mousePosition, Vector2.zero);
			if (hit.collider != null) {
				GameObject point = hit.collider.gameObject;
				int selectedLevel = System.Int32.Parse(point.name.Substring (LEVEL.Length));
				if (PlayerController.levelMaxAchieved >= selectedLevel) {
					PlayerController.levelNumber = selectedLevel;
					Destroy (targetPoint);
					MarkCurrentPoint (point);
				}
			}
		}
	}

}
