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


	void Update ()
	{
		
	}

}
