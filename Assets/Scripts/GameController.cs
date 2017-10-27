using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

	public GameObject figure;


	private int count = 20;

	void Start () {
		Respawn ();
	}
	

	public void Respawn() {
		if (count-- > 0) {
			Vector2 pos = ScriptManager.BoardController.getPos (BoardController.BOARD_WIDTH / 2 - 1, 1);
			Instantiate (figure, new Vector3 (pos.x, pos.y, 0), Quaternion.identity);
			figure.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -0.2f);
		}
	}

	void Update () {
		
	}

}
