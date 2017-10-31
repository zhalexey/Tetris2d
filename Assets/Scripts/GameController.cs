using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

	private int count = 100;

	void Start ()
	{
		Respawn ();
	}


	public void Respawn ()
	{
		if (count-- > 0) {
			Vector2 pos = ScriptManager.BoardController.getPos (BoardController.BOARD_WIDTH / 2 - 1, 1);

			var figure = GetNextFigure ();

			Instantiate (figure, new Vector3 (pos.x, pos.y, 0), Quaternion.identity);
			figure.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -0.2f);
		}
	}


	private GameObject GetNextFigure ()
	{
		return ScriptManager.BoardController.GetNextFigure ();
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


	void Update ()
	{
		
	}

}
