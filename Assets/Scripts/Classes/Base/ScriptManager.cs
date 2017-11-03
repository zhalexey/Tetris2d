using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager {

	public static BoardController BoardController
	{
		get{
			return (BoardController)GameObject.Find ("BoardManager").GetComponent ("BoardController");
		}
	}

	public static GameController GameController
	{
		get {
			return ((GameController)(GameObject.Find ("GameManager").GetComponent ("GameController")));
		}
	}

	public static SoundController SoundController
	{
		get {
			return ((SoundController)(GameObject.Find ("SoundManager").GetComponent ("SoundController")));
		}
	}

	public static AudioSource MusicAudioSource
	{
		get {
			return GameObject.Find ("Camera").GetComponent<AudioSource>();
		}
	}

	public static LevelMenuController LevelMenuController
	{
		get {
			return ((LevelMenuController)(GameObject.Find ("LevelMenuManager").GetComponent ("LevelMenuController")));
		}
	}

}
