using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class BurnBrickHelper
{
	public static BurnBrickHelper instance = new BurnBrickHelper ();

	private static int figureCounter = 0;
	private int coinCounter;
	private const string NEW_FIGURE_NAME = "Figure_divided_";
	private const string HIT_FIGURE_NAME = "Hit_figure_";
	private const float TIME_BEFORE_DESTROY_FIGURE = 1;
	private const int FIGURE_DROP_FORCE_SIDE = 100;
	private const int FIGURE_DROP_FORCE_DOWN = 300;
	private GameObject dropCointSFX;
	private GameObject targetObject;
	private BoardController.CountCoinDelegate callback;


	public int BurnBrickLine (Collider2D[] hits, GameObject dropCointSFX, GameObject targetObject, BoardController.CountCoinDelegate callback)
	{
		this.dropCointSFX = dropCointSFX;
		this.targetObject = targetObject;
		this.callback = callback;

		coinCounter = 0;
		foreach (Collider2D hit in hits) {
			BurnBrick (hit.gameObject);
		}
		ScriptManager.SoundController.PlaySound (SoundController.SoundAction.BurnLine);

		return coinCounter;
	}


	public void BurnBrick (GameObject hitBrick)
	{

		List<Group> groups = GetGroups (hitBrick);
		List<GameObject> figures = DivideFigure (groups);
		CleanBrickJoints (figures);
		PerformActionsForHit (figures);
	}


	private void PerformActionsForHit (List<GameObject> figures)
	{
		foreach (GameObject figure in figures) {
			if (figure.tag == BoardController.HIT_TAG) {
				PushFigure (figure);
			}
		}
	}


	private void PushFigure(GameObject figure) {
		Transform[] childs = figure.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			GameObject brick = child.gameObject;
			if (brick == figure) {
				continue;
			}

			brick.GetComponent<Collider2D> ().enabled = false;

			if (brick.tag == BoardController.COIN_TAG) {
				coinCounter++;
				GameObject dropCoinSXF = MonoBehaviour.Instantiate (dropCointSFX, brick.transform.position, Quaternion.identity);
				var dropCoinSFXController = dropCoinSXF.GetComponent<DropCoinSFXController> ();
				dropCoinSFXController.SetInitialData (targetObject, brick.GetComponent<SpriteRenderer>().sprite, callback);
				dropCoinSFXController.StartEffect ();
				MonoBehaviour.Destroy (figure);

			} else {
				Vector2 force = new Vector2 (FIGURE_DROP_FORCE_SIDE * Random.Range (-1, 2), FIGURE_DROP_FORCE_DOWN * Random.value);
				brick.GetComponent<Rigidbody2D> ().AddForce (force);
				MonoBehaviour.Destroy (figure, TIME_BEFORE_DESTROY_FIGURE);
			}
		}
	}


	private void CleanBrickJoints (List<GameObject> figures)
	{
		foreach (GameObject figure in figures) {
			Transform[] childs = figure.GetComponentsInChildren<Transform> ();
			foreach (Transform child in childs) {
				if (child == figure) {
					continue;
				}

				FixedJoint2D joint = child.GetComponent<FixedJoint2D> ();
				if (joint != null && (joint.connectedBody == null || joint.connectedBody.transform.parent != figure.transform)) {
					MonoBehaviour.Destroy (joint);
				}
			}
		}
	}


	private List<GameObject> DivideFigure (List<Group> groups)
	{
		List<GameObject> figures = new List<GameObject> ();
		GameObject parentFigure = null;

		foreach (Group group in groups) {

			GameObject newFigure = new GameObject ();

			if (group.IsHitGroup ()) {
				newFigure.tag = BoardController.HIT_TAG;
				newFigure.name = HIT_FIGURE_NAME + figureCounter;
			} else {
				newFigure.name = NEW_FIGURE_NAME + figureCounter;
			}
			figureCounter++;

			for (int i = 0; i < group.Count (); i++) {
				newFigure.AddComponent<FixedJoint2D> ();
			}

			FixedJoint2D[] joints = newFigure.GetComponents<FixedJoint2D> ();
			int j = 0;
			List<GameObject> bricks = group.List ();
			foreach (GameObject brick in bricks) {
				if (parentFigure == null) {
					parentFigure = brick.transform.parent.gameObject;
				}

				brick.transform.parent = newFigure.transform;
				joints [j++].connectedBody = brick.GetComponent<Rigidbody2D> ();
			}

			figures.Add (newFigure);
		}

		MonoBehaviour.Destroy (parentFigure);
		return figures;
	}

	public static Transform[] GetBricks (GameObject figure)
	{
		Transform[] brickTransforms = figure.GetComponentsInChildren<Transform> ();
		Transform[] brickTransformsCleared = new Transform[brickTransforms.Length - 1];
		int i = 0;
		foreach (Transform child in figure.GetComponentsInChildren<Transform> ()) {
			if (!child.name.Contains ("Figure")) {
				brickTransformsCleared [i++] = child;
			}
		}
		return brickTransformsCleared;
	}

	public static void SortBricksDescending(Transform[] bricks) {
		Array.Sort (bricks, DescendingByName);
	}

	private List<Group> GetGroups (GameObject hitBrick)
	{
		List<Group> groups = new List<Group> ();

		GameObject figure = hitBrick.transform.parent.gameObject;

		var brickTransforms = GetBricks (figure);
		Array.Sort (brickTransforms, AscendingByName);

		Group group = new Group ();
		foreach (Transform tr in brickTransforms) {
			GameObject brick = tr.gameObject;

			if (brick == figure) {
				continue;
			}

			if (brick == hitBrick) {
				if (group.Count () > 0) {
					groups.Add (group);
				}

				groups.Add (GetHitBrickGroup (hitBrick));
				group = new Group ();
				continue;
			}
			group.Add (brick);
		}

		if (!group.IsEmpty ()) {
			groups.Add (group);
		}

		return groups;
	}


	private Group GetHitBrickGroup (GameObject hitBrick)
	{
		Group group = new Group ();
		group.Add (hitBrick);
		group.SetHitGroup ();
		return group;
	}


	private int AscendingByName (Transform obj1, Transform obj2)
	{
		int num1 = GetBrickNumber (obj1.name);
		int num2 = GetBrickNumber (obj2.name);
		return num1 - num2;
	}

	private static int DescendingByName (Transform obj1, Transform obj2)
	{
		int num1 = GetBrickNumber (obj1.name);
		int num2 = GetBrickNumber (obj2.name);
		return num2 - num1;
	}


	private static int GetBrickNumber(string name) {
		int startIdx = name.IndexOf ("(") + 1;
		return Int32.Parse(name.Substring (startIdx, name.Length - startIdx - 1	));
	}

	
}

