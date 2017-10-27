using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameObjectController : MonoBehaviour
{


	void FixedUpdate ()
	{
		PreventBouncing ();
	}

	/*
	 * Prevent game object from bouncing
	 */
	private void PreventBouncing ()
	{
		Vector2 currentVelocity = gameObject.GetComponent<Rigidbody2D> ().velocity;
		if (currentVelocity.y <= 0f) {
			return;
		}
		currentVelocity.y = 0f;
		gameObject.GetComponent<Rigidbody2D> ().velocity = currentVelocity;
	}



	//--------------------------------------------------------------------

	protected void DebugBox (Vector2 pos)
	{
		Vector2 pointA = new Vector2 (pos.x - BoardController.BRICK_SIZE, pos.y + BoardController.BRICK_SIZE);
		Vector2 pointB = new Vector2 (pos.x + BoardController.BRICK_SIZE, pos.y - BoardController.BRICK_SIZE);
		Debug.DrawLine (pointA, pointB, Color.blue);
	}
}
