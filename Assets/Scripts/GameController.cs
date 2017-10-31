using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{

	private enum State
	{
		Started,
		Finished
	}

	private State state;

	private int count = 100;


	void Start ()
	{
		state = State.Started;

		Respawn ();
	}


	public void Respawn ()
	{
		if (count-- == 0) {
			state = State.Finished;
		}
			

		if (State.Started == state) {
			bool isRespawnAllowed = ScriptManager.BoardController.Respawn ();
			if (!isRespawnAllowed) {
				state = State.Finished;
			}
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
