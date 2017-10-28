using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : BaseGameObjectController
{

	enum FigureState
	{
		Idle,
		Rotation,
		MoveLeft,
		MoveRight,
		MoveDown,
		Stopped}

	;

	private FigureState state = FigureState.Idle;


	private float angle = 0;
	private float desiredAngle = 0;

	private float deltaH;
	private float maxDeltaH;
	private float minDeltaH = 1.0f;
	private float DX = 0.028f;
	public const float FALLING_VELOCITY = -0.2f;
	private const float FAST_FALLING_VELOCITY = -2f;


	private float desiredPosX;

	void Update ()
	{

		BoardGizmos.DrawBoxAim (gameObject.transform.position);


		if (FigureState.Stopped == state) {
			return;
		}

		if (FigureState.Idle == state || FigureState.MoveDown == state) {

			CheckFalled ();

			if (Input.GetKey (KeyCode.LeftArrow)) {
				if (ScriptManager.BoardController.CanMoveLeft (gameObject)) {
					Vector2 pos = gameObject.transform.position;
					desiredPosX = pos.x - BoardController.BRICK_SIZE;
					state = FigureState.MoveLeft;
				}

			} else if (Input.GetKey (KeyCode.RightArrow)) {
				if (ScriptManager.BoardController.CanMoveRight (gameObject)) {
					Vector2 pos = gameObject.transform.position;
					desiredPosX = pos.x + BoardController.BRICK_SIZE;
					state = FigureState.MoveRight;
				}

			} else if (Input.GetKey (KeyCode.UpArrow)) {
				if (FigureState.Idle == state && ScriptManager.BoardController.CanRotate (gameObject)) {

					SetFigureTrigger (true);
					desiredAngle += 90;
					state = FigureState.Rotation;
					deltaH = gameObject.transform.position.y;
				}

			} else if (Input.GetKey (KeyCode.DownArrow)) {
				if (ScriptManager.BoardController.CanMoveDown (gameObject)) {
					state = FigureState.MoveDown;
				}
			} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
				if (FigureState.MoveDown == state) {
					state = FigureState.Idle;
				}

			} else if (Input.GetKeyDown (KeyCode.Space)) {
				gameObject.transform.position = ScriptManager.BoardController.getPos (BoardController.BOARD_WIDTH / 2, 1);
			}
		}

		PerformTransformation ();
	}


	protected void CheckFalled ()
	{
		if (ScriptManager.BoardController.CanNotMoveDown (gameObject)) {
			state = FigureState.Stopped;
			ResetFigureTags ();
			Destroy (this);
			ScriptManager.GameController.Respawn ();
		}
	}


	//---------------------------------------------------- transform figure ---------------------------------------------

	void PerformTransformation ()
	{
		if (FigureState.Rotation == state) {
			PerformRotation ();

		} else if (FigureState.MoveLeft == state) {
			PerformMoveLeft ();

		} else if (FigureState.MoveRight == state) {
			PerformMoveRight ();
		}

		PerformFallingDown ();
	}

	void PerformRotation ()
	{
		if (angle < desiredAngle) {
			angle += 10;
			gameObject.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			if (angle == 360) {
				angle = 0;
				desiredAngle = 0;
			}
		} else {
			StopRotation ();
		}
	}

	void StopRotation ()
	{
		SetFigureTrigger (false);
		state = FigureState.Idle;
		//BoardGizmos.ClearLog ();
		float delta = deltaH - gameObject.transform.position.y;
		if (delta > maxDeltaH) {
			maxDeltaH = delta;
		}
		if (delta < minDeltaH) {
			minDeltaH = delta;
		}
		//Debug.Log ("max=" + maxDeltaH + "  min=" + minDeltaH);
	}

	void PerformMoveLeft ()
	{
		Vector2 position = gameObject.transform.position;
		if (position.x > desiredPosX) {
			position.x -= DX;
			gameObject.transform.position = position;
		} else {
			position.x = desiredPosX;
			gameObject.transform.position = position;
			state = FigureState.Idle;
		}
	}

	void PerformMoveRight ()
	{
		Vector2 position = gameObject.transform.position;
		if (position.x < desiredPosX) {
			position.x += DX;
			gameObject.transform.position = position;
		} else {
			position.x = desiredPosX;
			gameObject.transform.position = position;
			state = FigureState.Idle;
		}
	}

	void PerformFallingDown ()
	{
		Vector2 velocity = new Vector2 (0, state == FigureState.MoveDown ? FAST_FALLING_VELOCITY : FALLING_VELOCITY);
		gameObject.GetComponent<Rigidbody2D> ().velocity = velocity;
	}

	//--------------------------------------------------------------------------------------

	void SetFigureTrigger (bool isTrigger)
	{
		Transform[] childs = gameObject.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			if (child.gameObject.GetInstanceID () != gameObject.GetInstanceID ()) {
				child.GetComponent<Collider2D> ().isTrigger = isTrigger;
			}
		}
	}

	void ResetFigureTags ()
	{
		Transform[] childs = gameObject.transform.GetComponentsInChildren<Transform> ();
		foreach (Transform child in childs) {
			child.tag = BoardController.UNTAGGED;
			child.GetComponent<Rigidbody2D> ().gravityScale = 0.125f;
		}
	}

}
