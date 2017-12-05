using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : BaseGameObjectController
{

	public enum RotationType {
		FREE_ROTATE, TWO_DIMENSION, STATIC
	}

	enum FigureState
	{
		Idle,
		Rotation,
		MoveLeft,
		MoveRight,
		MoveDown,
		MoveDownFast,
		Stopped}

	;

	public RotationType rotationType;


	private const float GRAVITY = 0.125f;
	private const int ROTATION_DELTA = 10;
	private const float DELTA_X = 0.036f;
	public const float FALLING_VELOCITY = -0.2f;
	private const float FAST_FALLING_VELOCITY = -3f;
	private const float VERY_FAST_FALLING_VELOCITY = -5f;

	private FigureState state = FigureState.Idle;
	private float angle = 0;
	private float desiredAngle = 0;
	private bool rotateBack = false;

	private float deltaH;
	private float maxDeltaH;
	private float minDeltaH = 1.0f;


	private float desiredPosX;

	void Update ()
	{
		if (FigureState.Stopped == state) {
			return;
		}

		// Level menu invokation
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ScriptManager.GameController.PauseResume ();
		}


		if (isPause) {
			return;
		}

		if (FigureState.MoveDownFast == state) {
			CheckFalled ();
			PerformTransformation ();
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
					ApplyRotateRestriction ();
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
				if (ScriptManager.BoardController.CanMoveDown (gameObject)) {
					state = FigureState.MoveDownFast;
				}
			}
		}

		PerformTransformation ();
	}


	private void ApplyRotateRestriction ()
	{
		if (RotationType.STATIC != rotationType) {

			if (RotationType.TWO_DIMENSION == rotationType) {
				if (angle == 0) {
					desiredAngle = 90;
				}
				else {
					rotateBack = true;
					desiredAngle = 0;
				}
			}
			else
				if (RotationType.FREE_ROTATE == rotationType) {
					desiredAngle += 90;
				}
			
			SetFigureTrigger (true);
			state = FigureState.Rotation;
			deltaH = gameObject.transform.position.y;

			ScriptManager.SoundController.PlaySound (SoundController.SoundAction.Rotate);
		}
	}


	protected void CheckFalled ()
	{
		if (ScriptManager.BoardController.CanNotMoveDown (gameObject)) {
			state = FigureState.Stopped;
			ResetFigureTags ();
			Destroy (this);
			ScriptManager.SoundController.PlaySound (SoundController.SoundAction.Falling);
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
		if (rotateBack) {
			PerformRotateBack ();
		} else {
			PerformRotateNormal ();
		}
	}


	void PerformRotateBack ()
	{
		if (angle > desiredAngle) {
			angle -= ROTATION_DELTA;
			gameObject.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
		}
		else {
			StopRotation ();
			rotateBack = false;
		}
	}


	void PerformRotateNormal ()
	{
		if (angle < desiredAngle) {
			angle += ROTATION_DELTA;
			gameObject.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
			if (angle == 360) {
				angle = 0;
				desiredAngle = 0;
			}
		}
		else {
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
			position.x -= DELTA_X;
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
			position.x += DELTA_X;
			gameObject.transform.position = position;
		} else {
			position.x = desiredPosX;
			gameObject.transform.position = position;
			state = FigureState.Idle;
		}
	}

	void PerformFallingDown ()
	{
		float velocityY = 0f;
		if (FigureState.MoveDown == state) {
			velocityY = FAST_FALLING_VELOCITY;
		} else if (FigureState.MoveDownFast == state) {
			velocityY = VERY_FAST_FALLING_VELOCITY;
		} else {
			velocityY = FALLING_VELOCITY;
		}

		Vector2 velocity = new Vector2 (0, velocityY);
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
			child.GetComponent<Rigidbody2D> ().gravityScale = GRAVITY;
		}
	}

}
