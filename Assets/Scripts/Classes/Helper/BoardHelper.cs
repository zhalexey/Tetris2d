﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardHelper
{

	private static readonly BoardHelper instance = new BoardHelper();

	static BoardHelper() {
	}

	private BoardHelper() {
	}

	public static BoardHelper Instance {
		get {
			return instance;
		}
	}


	public int BurnBrickLine (Collider2D[] hits)
	{
		int hitCoins = 0;

		List<GameObject> parents = new List<GameObject> ();
		List<GameObject> toDestroy = new List<GameObject> ();
		List<FixedJoint2D> toDestroyJoints = new List<FixedJoint2D> ();

		foreach (Collider2D hit in hits) {

			// 1. Mark brick to destroy
			toDestroy.Add (hit.gameObject);
			if (BoardController.COIN_TAG == hit.gameObject.tag) {
				hitCoins++;
			}

			// 2. Mark parent figure joints to destroy if it is empty
			GameObject parent = hit.gameObject.transform.parent.gameObject;
			toDestroyJoints.AddRange(GetParentJointsToDestroy (hit, parent));

			Transform[] childObjs = parent.GetComponentsInChildren<Transform> ();

			// First is the figure itself, second is the hit brick
			if (childObjs.Length == 2) {
				toDestroy.Add (parent);
			} else {
				parents.Add (parent);
			}

		}

		// 3. Destroy marked 
		foreach (GameObject obj in toDestroy) {
			FireBrickSpecialEffect (obj);
			MonoBehaviour.DestroyImmediate(obj);
			ScriptManager.SoundController.PlaySound (SoundController.SoundAction.BurnLine);
		}

		// 4. Destroy empty figure`s joints
		foreach (FixedJoint2D joint in toDestroyJoints) {
			MonoBehaviour.Destroy (joint);
		}


		// 5. Divide into figures
		DivideFigures (parents);

		return hitCoins;
	}


	private List<FixedJoint2D> GetParentJointsToDestroy (Collider2D hit, GameObject parent)
	{
		List<FixedJoint2D> joints = new List<FixedJoint2D> ();
		FixedJoint2D[] parentJoints = parent.GetComponents<FixedJoint2D> ();
		foreach (FixedJoint2D joint in parentJoints) {
			Rigidbody2D childBrickBody = joint.connectedBody;
			if (childBrickBody != null && childBrickBody.gameObject == hit.gameObject) {
				joints.Add (joint);
			}
		}
		return joints;
	}

	private void FireBrickSpecialEffect (GameObject obj)
	{
		// ignore parent figure
		if (obj.name.Contains ("Figure")) {
			return;
		}

		GameObject effect = MonoBehaviour.Instantiate (ScriptManager.BoardController.brickBurnEffect, obj.transform.position, Quaternion.identity);

		Material material = effect.GetComponent<ParticleSystemRenderer> ().material;
		Texture2D texture = obj.GetComponent<SpriteRenderer> ().sprite.texture;
		material.SetTexture ("_MainTex", texture);

		MonoBehaviour.Destroy (effect, effect.GetComponent<ParticleSystem> ().main.duration);
	}


	private void DivideFigures (List<GameObject> figures)
	{
		if (figures.Count == 0) {
			return;
		}
		foreach (GameObject figure in figures) {
			DivideFigure (figure);
		}
	}


	private void DivideFigure (GameObject figure)
	{
		if (figure == null) {
			return;
		}

		if (isPossibleToDivideFigure (figure)) {
			
			List<Group> groups = DivideIntoGroups (figure);

			// Groups more than one - its a reason to divide figure
			if (groups.Count > 1) {
				CreateNewFigures (figure, groups);
			}
			return;
		}

		RemoveEmptyJoints (figure);
	}


	private void RemoveEmptyJoints(GameObject figure) {
		Transform[] childs = figure.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			FixedJoint2D joint = child.GetComponent<FixedJoint2D> ();
			if (joint != null && joint.connectedBody == null) {
				MonoBehaviour.Destroy (joint);
			}
		}
	}


	private bool isPossibleToDivideFigure(GameObject figure) {
		return figure.GetComponentsInChildren<Transform>().Length > 2;
	}

	private int ByNameComparison (Transform obj1, Transform obj2)
	{
		return obj1.name.CompareTo (obj2.name);
	}


	private List<Group> DivideIntoGroups (GameObject figure)
	{
		List<FixedJoint2D> jointsToDestroy = new List<FixedJoint2D> ();

		Transform[] childs = figure.GetComponentsInChildren<Transform> ();
		Array.Sort (childs, ByNameComparison);

		List<Group> groups = new List<Group> ();
		List<GameObject> bricks = new List<GameObject> ();

		foreach (Transform child in childs) {
			if (figure != child.gameObject) {
				
				bricks.Add (child.gameObject);
				FixedJoint2D joint = child.gameObject.GetComponent<FixedJoint2D> ();

				if (joint != null) {
					Rigidbody2D connectedBody = joint.connectedBody;
					if (connectedBody == null) {
						groups.Add (new Group (bricks));
						bricks = new List<GameObject> ();
						jointsToDestroy.Add (joint);
					}
				} else {
					groups.Add (new Group (bricks));
					bricks = new List<GameObject> ();
				}
			}
		}

		if (bricks.Count > 0) {
			groups.Add(new Group(bricks));
		}

		// Destroy empty joints
		foreach (FixedJoint2D joint in jointsToDestroy) {
			MonoBehaviour.Destroy (joint);
		}

		return groups;
	}


	private List<FixedJoint2D> CreateNewFigures (GameObject figure, List<Group> groups)
	{
		List<FixedJoint2D> jointsToMove = new List<FixedJoint2D> ();
		FixedJoint2D[] parentFigureJoints = figure.GetComponents<FixedJoint2D> ();
		int count = 0;
		foreach (Group group in groups) {
			if (count++ > 0) {
				foreach (GameObject brick in group.Values) {
					foreach (FixedJoint2D parentJoint in parentFigureJoints) {
						Rigidbody2D connectedBody = parentJoint.connectedBody;
						if (connectedBody != null && brick == connectedBody.gameObject) {
							jointsToMove.Add (parentJoint);
						}
					}
				}

				CreateNewFigure(jointsToMove);
				jointsToMove = new List<FixedJoint2D> ();
			}
		}

		return jointsToMove;
	}


	/*
	 * Create new figures from moved joints` gameobjects
	 */
	private void CreateNewFigure (List<FixedJoint2D> jointsToMove)
	{
		GameObject newFigure = new GameObject ("Figure_divided");
		newFigure.transform.position = CountAveragePosition (jointsToMove);

		for (int i = 0; i < jointsToMove.Count; i++) {
			newFigure.AddComponent<FixedJoint2D> ();
		}

		List<FixedJoint2D> jointsToDestroy = new List<FixedJoint2D>();

		FixedJoint2D[] newJoints = newFigure.GetComponents<FixedJoint2D> ();
		int j = 0;
		foreach (FixedJoint2D joint in jointsToMove) {
			joint.connectedBody.gameObject.transform.parent = newFigure.transform;
			newJoints [j++].connectedBody = joint.connectedBody;
			jointsToDestroy.Add (joint);
		}

		foreach (FixedJoint2D joint in jointsToDestroy) {
			MonoBehaviour.Destroy (joint);
		}
	}


	/*
	 * Count average position of joints' objects
	 */
	private Vector3 CountAveragePosition (List<FixedJoint2D> toDestroyJoints)
	{
		// Get new figure position
		Vector3 averagePosition = new Vector3 (0, 0, 0);
		foreach (FixedJoint2D joint in toDestroyJoints) {
			averagePosition += joint.gameObject.transform.position;
		}
		averagePosition /= toDestroyJoints.Count;
		return averagePosition;
	}



}
