using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DropCoinSFXController : MonoBehaviour
{
	private enum State
	{
		Started,
		Finished
	}

	private const bool RANDOMIZE_SPEED = true;
	private const float RANDOMIZE_SPEED_MIN = 2f;
	private const float RANDOMIZE_SPEED_MAX = 4f;
	private const float SPEED = 4f;
	private const float Z_POS = 0;
	private const float PERIOD = 0.5f;
	private const float AMPLITUDE = 0.05f;
	private const float TARGET_ACCURACY = 0.2f;

	private State state;
	private GameObject targetObject;
	private int sign;
	private float speed;
	private Action callback;



	public void SetInitialData (GameObject targetObject, Sprite sprite, Action callback)
	{
		this.targetObject = targetObject;
		this.callback = callback;

		SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer> ();
		spriteRenderer.sprite = sprite;
		spriteRenderer.sortingLayerName = BoardController.FOREGROUND_LAYER;

		ParticleSystem particleSystem = gameObject.GetComponentInChildren<ParticleSystem> ();
		particleSystem.GetComponent<Renderer> ().sortingLayerName = BoardController.FOREGROUND_LAYER;
	}


	public void StartEffect ()
	{
		sign = targetObject.transform.position.x - gameObject.transform.position.x > 0 ? 1 : -1;
		speed = RANDOMIZE_SPEED ? UnityEngine.Random.Range (RANDOMIZE_SPEED_MIN, RANDOMIZE_SPEED_MAX) : SPEED;
		state = State.Started;
	}


	void Update ()
	{

		if (state == State.Finished) {
			return;
		}

		if (TargetAchieved ()) {
			state = State.Finished;
			gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = null;
			Destroy (gameObject, gameObject.GetComponentInChildren<ParticleSystem> ().main.duration);
			callback ();
			return;
		}



		float dx = speed * Time.deltaTime * sign;

		float newX = gameObject.transform.position.x + dx;
		float newY = Func (newX) + Mathf.Cos (Time.time * PERIOD) * AMPLITUDE;
		gameObject.transform.position = new Vector3 (newX, newY, Z_POS);
	}



	private bool TargetAchieved ()
	{
		return GetDistance () <= TARGET_ACCURACY;
	}


	private float GetDistance ()
	{
		Vector2 point1 = gameObject.transform.position;
		Vector2 point2 = targetObject.transform.position;
		return Vector2.Distance (point1, point2);
	}


	private float Func (float x)
	{
		Vector2 point1 = gameObject.transform.position;
		Vector2 point2 = targetObject.transform.position;

		float a = point2.x - point1.x;
		float b = point2.y - point1.y;
		float c = b / a;

		float dy = c * (x - point1.x) + point1.y;
		return dy;
	}

}


