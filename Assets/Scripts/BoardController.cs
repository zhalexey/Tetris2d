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

	public GameObject figureBoardTest;

	public List<GameObject> figures;

	private Vector2 initPosition;


	void Start ()
	{
		initPosition = getPos (new Vector2 (BoardController.BOARD_WIDTH / 2 - 1, 0));
		//Instantiate (figureBoardTest, new Vector3 (0, -1.8f, 0), Quaternion.identity);
	}


	void Update ()
	{
		CheckBoardStatus ();
	}


	//--------------------------------------------- Next figure ------------------------------------------



	private GameObject GetNextFigure() {
		return figures [UnityEngine.Random.Range(0, figures.Count)];
	}


	private GameObject GetNextFigure_Debug ()
	{
		GameObject figure;
		do {
			figure = ScriptManager.BoardController.GetNextFigure ();
		} while (!ContainsName (figure.name, new string[2]{ "Figure_Z", "Figure_I" }));
		return figure;
	}


	private bool ContainsName (string comparable, string[] values)
	{
		foreach (string value in values) {
			if (comparable.Equals (value)) {
				return true;
			}
		}
		return false;
	}


	public bool Respawn ()
	{
		var figure = GetNextFigure ();
		if (!CanBePlaced (figure, initPosition)) {
			return false;
		}
		Instantiate (figure, new Vector3 (initPosition.x, initPosition.y, 0), Quaternion.identity);
		figure.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, FigureController.FALLING_VELOCITY);
		return true;
	}
	//--------------------------------------------- Board instrument methods -----------------------------


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


	public Vector2 getPos (Vector2 pos)
	{
		return getPos ((int)pos.x, (int)pos.y);
	}


	/*
	 * Get board position from absolute position
	 */
	private Vector2 GetBoardPos (Vector2 absPos)
	{
		return new Vector2 (BOARD_HALF_WIDTH + absPos.x / BRICK_SIZE, BOARD_HALF_HEIGHT - 1 - absPos.y / BRICK_SIZE);
	}

	//----------------------------------------------------------Check board status--------------------------------------

	void CheckBoardStatus ()
	{	
		
		for (int i = BOARD_HEIGHT; i > 0; i--) {
			int counter = 0;

			Vector2 pointA = getPos (0, i);
			Vector2 pointB = getPos (BOARD_WIDTH - 1, i);

			Collider2D[] hits = Physics2D.OverlapAreaAll (pointA, pointB);

			if (BOARD_WIDTH == hits.Length) {
				foreach (Collider2D hit in hits) {
					if (isNotPlayerCollider (hit) && isFigureHaveFalled (hit)) {
						counter++;
					}
				}

				if (counter == BOARD_WIDTH) {
					BoardHelper.Instance.BurnBrickLine (hits);
				}

			}
		}
	}


	private bool isNotPlayerCollider (Collider2D hit)
	{
		return hit.tag != PLAYER_TAG;
	}


	private bool isFigureHaveFalled (Collider2D collider)
	{
		return collider.gameObject.GetComponent<Rigidbody2D> ().velocity.y > FigureController.FALLING_VELOCITY;
	}


	//---------------------------------------------------------- Figure validations --------------------------------------


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


	public bool CanBePlaced (GameObject figure, Vector2 pos)
	{
		GameObject figureClone = Instantiate (figure);
		figureClone.transform.position = pos;

		if (CanFigureStay (figureClone)) {
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
