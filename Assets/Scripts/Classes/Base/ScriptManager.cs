using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager {

	public const string BOARD_MANAGER = "BoardManager";
	public const string BOARD_CONTROLLER = "BoardController";
	public const string GAME_MANAGER = "GameManager";
	public const string GAME_CONTROLLER = "GameController";
	public const string LEVEL_MENU_MANAGER = "LevelMenuManager";
	public const string LEVEL_MENU_CONTROLLER = "LevelMenuController";
	public const string SOUND_MANAGER = "SoundManager";
	public const string SOUND_CONTROLLER = "SoundController";
	public const string LEVEL_CONFIG = "LevelConfig";
	public const string LEVEL_CONFIG_CONTROLLER = "LevelConfigController";


	public static BoardController BoardController
	{
		get{
			return (BoardController)GameObject.Find (BOARD_MANAGER).GetComponent (BOARD_CONTROLLER);
		}
	}

	public static GameController GameController
	{
		get {
			return ((GameController)(GameObject.Find (GAME_MANAGER).GetComponent (GAME_CONTROLLER)));
		}
	}

	public static SoundController SoundController
	{
		get {
			return ((SoundController)(GameObject.Find (SOUND_MANAGER).GetComponent (SOUND_CONTROLLER)));
		}
	}

	public static LevelMenuController LevelMenuController
	{
		get {
			return ((LevelMenuController)(GameObject.Find (LEVEL_MENU_MANAGER).GetComponent (LEVEL_MENU_CONTROLLER)));
		}
	}

	public static LevelConfigController LevelConfigController
	{
		get {
			return ((LevelConfigController)(GameObject.Find (LEVEL_CONFIG).GetComponent (LEVEL_CONFIG_CONTROLLER)));
		}
	}

}
