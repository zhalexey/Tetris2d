using System;
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


	public List<GameObject> BurnBrickLine (Collider2D[] hits)
	{
		List<GameObject> parents = new List<GameObject> ();
		List<GameObject> toDestroy = new List<GameObject> ();
		List<FixedJoint2D> toDestroyJoints = new List<FixedJoint2D> ();

		foreach (Collider2D hit in hits) {

			// 1. Mark brick to destroy
			toDestroy.Add (hit.gameObject);

			// 2. Mark parent figure to destroy if it is empty
			GameObject parent = hit.gameObject.transform.parent.gameObject;

			// 2.5 Mark fixed joints of parent to destroy
			FixedJoint2D[] parentJoints = parent.GetComponents<FixedJoint2D> ();
			foreach (FixedJoint2D joint in parentJoints) {
				if (joint.connectedBody.gameObject.Equals(hit.gameObject)) {
					toDestroyJoints.Add(joint);
				}
			}

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
			MonoBehaviour.DestroyImmediate (obj);
		}

		// 4. Destroy empty figure`s joints
		foreach (FixedJoint2D joint in toDestroyJoints) {
			MonoBehaviour.Destroy (joint);
		}
		

		return parents;
	}


	public void DivideFigures (List<GameObject> figures)
	{
		if (figures.Count == 0) {
			return;
		}
		foreach (GameObject figure in figures) {
			DivideFigure (figure);
		}
	}


	void DivideFigure (GameObject figure)
	{
		if (figure == null) {
			return;
		}

		List<Group> groups = DivideIntoGroups (figure);

		// Groups more than one - its a reason to divide figure
		if (groups.Count > 1) {
			List<FixedJoint2D> toDestroyJoints = CollectJointsToDestroy (figure, groups);
			CreateNewFigure (toDestroyJoints);
		}
	}


	private int ByNameComparison (Transform obj1, Transform obj2)
	{
		return obj1.name.CompareTo (obj2.name);
	}


	private List<Group> DivideIntoGroups (GameObject figure)
	{
		List<FixedJoint2D> emptyJoints = new List<FixedJoint2D> ();

		Transform[] childs = figure.GetComponentsInChildren<Transform> ();
		Array.Sort (childs, ByNameComparison);

		List<Group> groups = new List<Group> ();
		List<GameObject> bricks = new List<GameObject> ();
		foreach (Transform child in childs) {
			if (child.gameObject.GetInstanceID () != figure.GetInstanceID ()) {
				bricks.Add (child.gameObject);
				FixedJoint2D joint = child.gameObject.GetComponent<FixedJoint2D> ();
				Rigidbody2D connectedBody = joint.connectedBody;
				if (connectedBody == null) {
					groups.Add (new Group (bricks));
					bricks = new List<GameObject> ();
					emptyJoints.Add (joint);
				}
			}
		}

		// Destroy empty joints
		foreach (FixedJoint2D joint in emptyJoints) {
			MonoBehaviour.Destroy (joint);
		}


		return groups;
	}


	private List<FixedJoint2D> CollectJointsToDestroy (GameObject figure, List<Group> groups)
	{
		List<FixedJoint2D> toDestroyJoints = new List<FixedJoint2D> ();
		FixedJoint2D[] parentFigureJoints = figure.GetComponents<FixedJoint2D> ();
		int count = 0;
		foreach (Group group in groups) {
			if (count++ > 0) {
				foreach (GameObject brick in group.Values) {
					foreach (FixedJoint2D parentJoint in parentFigureJoints) {
						Rigidbody2D connectedBody = parentJoint.connectedBody;
						if (connectedBody != null && brick.GetInstanceID ().Equals (connectedBody.gameObject.GetInstanceID ())) {
							toDestroyJoints.Add (parentJoint);
						}
					}
				}
			}
		}
		return toDestroyJoints;
	}


	/*
	 * Create new figures from destroyed joints` gameobjects
	 */
	private void CreateNewFigure (List<FixedJoint2D> toDestroyJoints)
	{
		GameObject newFigure = new GameObject ("Figure_divided");
		newFigure.transform.position = CountAveragePosition (toDestroyJoints);
		for (int i = 0; i < toDestroyJoints.Count; i++) {
			newFigure.AddComponent<FixedJoint2D> ();
		}
		FixedJoint2D[] newJoints = newFigure.GetComponents<FixedJoint2D> ();
		int j = 0;
		foreach (FixedJoint2D joint in toDestroyJoints) {
			joint.connectedBody.gameObject.transform.parent = newFigure.transform;
			newJoints [j].connectedBody = joint.connectedBody;
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
