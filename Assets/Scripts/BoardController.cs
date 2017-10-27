using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardController : MonoBehaviour
{
	

	public const int BOARD_WIDTH = 11;
	public const int BOARD_HALF_WIDTH = BOARD_WIDTH / 2;
	public const int BOARD_HEIGHT = 12;
	public static int BOARD_HALF_HEIGHT = BOARD_HEIGHT / 2;
	public static float BRICK_SIZE = 0.3f;
	public static float BRICK_HALF_SIZE = BRICK_SIZE / 2 - 0.02f;
	private const string PLAYER_TAG = "Player";
	public const string UNTAGGED = "Untagged";


	private const float MIN_DOWN_DISTANCE = 0.01f;

	private const float MIN_DELTA_H = 0.028f;
	private const float MAX_DELTA_H = 0.032f;

	public static bool[,] grid = new bool[BOARD_HEIGHT, BOARD_WIDTH];

	/*
	public static bool[,] grid = new bool[BOARD_HEIGHT, BOARD_WIDTH] {
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false, false, false, false, false, false, false, false, false, false, false},
		{ true,  true,  true, false,  true,  true,  true,  true,  true,  true,  true},
		{ true,  true,  true, false,  true,  true,  true,  true,  true,  true,  true},
		{false, false, false, false, false, false, false, false, false, false, false},
		{false,  false, false, false, false, false, false, false, false, false, false}
	};*/

	public GameObject brick;
	public GameObject figureBoardTest;


	int countHits;

	void Start ()
	{
		//Instantiate (figureBoardTest, new Vector3 (0, -1.8f, 0), Quaternion.identity);
		//InitBoard ();
		//DrawBoard ();
	}

	void InitBoard ()
	{
		for (int i = 0; i < BOARD_HEIGHT; i++)
			for (int j = 0; j < BOARD_WIDTH; j++) {
				grid [i, j] = UnityEngine.Random.value < 0.5;
			}
	}

	void DrawBoard ()
	{
		for (int i = 0; i < BOARD_HEIGHT; i++)
			for (int j = 0; j < BOARD_WIDTH; j++) {
				Vector2 pos = getPos (j, i);
				if (grid [i, j]) {
					Instantiate (brick, new Vector3 (pos.x, pos.y, 0), Quaternion.identity);
				}
			}
	}

	/*
	 * Get absolute position from board position 
	 */
	public Vector2 getPos (int x, int y)
	{
		Vector2 pos = new Vector2 ();
		pos.x = (x - BOARD_HALF_WIDTH) * BRICK_SIZE;
		pos.y = (-y + BOARD_HALF_HEIGHT - 1) * BRICK_SIZE;
		return pos;
	}

	public Vector2 getFloorPos (int x, int y)
	{
		Vector2 pos = new Vector2 ();
		pos.x = (x - BOARD_HALF_WIDTH) * BRICK_SIZE;
		pos.y = (-y + BOARD_HALF_HEIGHT - 1) * BRICK_SIZE - BRICK_HALF_SIZE;
		return pos;
	}

	/*
	 * Get board position from absolute position
	 */
	private Vector2 GetBoardPos (Vector2 absPos)
	{
		return new Vector2 (BOARD_HALF_WIDTH + absPos.x / BRICK_SIZE, BOARD_HALF_HEIGHT - 1 - absPos.y / BRICK_SIZE);
	}


	//----------------------------------------------------------Check board status--------------------------------------

	void Update ()
	{

		CheckBoardStatus ();

		//if (Input.GetKey (KeyCode.Space)) {
		//BoardGizmos.ClearLog ();
		//DebugGrid ();
		//}
	}


	void CheckBoardStatus ()
	{	
		
		for (int i = BOARD_HEIGHT; i > 0; i--) {
			int counter = 0;

			Vector2 pointA = getPos (0, i);
			Vector2 pointB = getPos (BOARD_WIDTH - 1, i);

			Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);


			if (hits.Length >= BOARD_WIDTH) {
				foreach (Collider2D hit in hits) {
					if (hit.tag != PLAYER_TAG) {
						counter++;
					}
				}

				if (counter == BOARD_WIDTH) {
					List<GameObject> parentFigures = BurnBrickLine (hits);
					DivideFigures (parentFigures);
				}

			}
		}
	}


	private List<GameObject> BurnBrickLine (Collider2D[] hits)
	{
		List<GameObject> parents = new List<GameObject> ();

		foreach (Collider2D hit in hits) {
			if (hit.tag != PLAYER_TAG) {
				List<GameObject> toDestroy = new List<GameObject> ();

				// 1. Mark brick to destroy
				toDestroy.Add (hit.gameObject);

				// 2. Mark parent figure to destroy if it is empty
				GameObject parent = hit.gameObject.transform.parent.gameObject;
				Transform[] childObjs = parent.GetComponentsInChildren<Transform> ();

				// First is the figure itself, second is the hit brick
				if (childObjs.Length == 2) {
					toDestroy.Add (parent);
				} else {
					parents.Add (parent);
				}

				// 3. Destroy marked 
				foreach (GameObject obj in toDestroy) {
					DestroyImmediate(obj);
				}
			}
		}
		return parents;
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


	void DivideFigure (GameObject figure)
	{
		if (figure == null) {
			return;
		}
			
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
					groups.Add(new Group(bricks));
					bricks = new List<GameObject> ();
				}

			}
		}


		// Groups more than one - its a reason to divide figure

		if (groups.Count > 1) {


			List<FixedJoint2D> toDestroyJoints = new List<FixedJoint2D> ();

			FixedJoint2D[] parentFigureJoints = figure.GetComponents<FixedJoint2D> ();

			int count = 0;
			foreach (Group group in groups) {
				if (count++ > 0) {

					foreach (GameObject brick in group.Values) {

						foreach (FixedJoint2D parentJoint in parentFigureJoints) {
						
							Rigidbody2D connectedBody = parentJoint.connectedBody;
							if (connectedBody != null && brick.GetInstanceID().Equals(connectedBody.gameObject.GetInstanceID())) {
								toDestroyJoints.Add (parentJoint);


							}

						}

					}

				}
			}


			// Get new figure position

			Vector3 averagePosition = new Vector3 (0, 0, 0);
			foreach (FixedJoint2D joint in toDestroyJoints) {
				averagePosition += joint.gameObject.transform.position;
			}
			averagePosition /= toDestroyJoints.Count;


			// Create new figures, add joints


			GameObject newFigure = new GameObject ("Figure_divided");
			newFigure.transform.position = averagePosition;

			for (int i = 0; i < toDestroyJoints.Count; i++) {
				newFigure.AddComponent<FixedJoint2D> ();
			}
			FixedJoint2D[] newJoints = newFigure.GetComponents<FixedJoint2D> ();

			int j = 0;
			foreach (FixedJoint2D joint in toDestroyJoints) {
				joint.connectedBody.gameObject.transform.parent = newFigure.transform;
					
				newJoints[j].connectedBody = joint.connectedBody;
				Destroy (joint);
			}

			Debug.Log("test");		
		}

	}


	private int ByNameComparison(Transform obj1, Transform obj2) {
		return obj1.name.CompareTo (obj2.name);
	}


	//------------------------------------------------------------------------------------------

	public void DebugGrid ()
	{
		for (int i = 0; i < BOARD_HEIGHT; i++) {
			string line = "";
			for (int j = 0; j < BOARD_WIDTH; j++) {
				line += grid [i, j] ? "1" : "0";
			}
			Debug.Log (line);
		}
	}








	//----------------------------------------------------------Figure validations--------------------------------------

	public bool CanRotate (GameObject figure)
	{
		return CanBePlaced (figure, new RotateAction ());
	}

	public bool CanMoveLeft (GameObject figure)
	{
		return CanBePlaced (figure, new MoveLeftAction ());
	}

	public bool CanMoveRight (GameObject figure)
	{
		return CanBePlaced (figure, new MoveRightAction ());
	}

	public bool CanMoveDown (GameObject figure)
	{
		return CanBePlaced (figure, new MoveDownAction ());
	}

	public bool CanNotMoveDown (GameObject figure)
	{
		GameObject figureClone = Instantiate (figure);
		Vector2 pos = figureClone.transform.position;
		pos.y -= MIN_DOWN_DISTANCE;
		figureClone.transform.position = pos;

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return true;
		}

		Destroy (figureClone);
		return false;
	}


	public bool CanBePlaced (GameObject figure, FigureAction action)
	{

		// first dh

		GameObject figureClone = Instantiate (figure);
		Vector2 pos = figureClone.transform.position;
		pos.y -= MIN_DELTA_H;
		figureClone.transform.position = pos;

		action.setObject (figureClone);
		action.performAction ();

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return false;
		}

		// second dh

		pos = figureClone.transform.position;
		pos.y -= (MAX_DELTA_H - MIN_DELTA_H);
		figureClone.transform.position = pos;

		if (!CanFigureStay (figureClone)) {
			Destroy (figureClone);
			return false;
		}

		Destroy (figureClone);
		return true;
	}


	private bool CanFigureStay (GameObject figure)
	{
		Transform[] childs = figure.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			if (child.gameObject.GetInstanceID () != figure.GetInstanceID ()) {
				if (!CanBrickStay (child.position)) {
					return false;
				}
			}
		}
		return true;
	}


	public bool CanBrickStay (Vector2 position)
	{
		Vector2 pointA = new Vector2 (position.x - BRICK_HALF_SIZE, position.y + BRICK_HALF_SIZE);
		Vector2 pointB = new Vector2 (position.x + BRICK_HALF_SIZE, position.y - BRICK_HALF_SIZE);

		Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);
		foreach (Collider2D hit in hits) {
			if (PLAYER_TAG != hit.tag) {
				return false;
			}
		}
		return true;
	}

}
