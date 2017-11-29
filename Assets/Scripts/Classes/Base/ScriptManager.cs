using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager {

	public const string BOARD_CONTROLLER = "BoardController";
	public const string GAME_CONTROLLER = "GameController";
	public const string LEVEL_MENU_CONTROLLER = "LevelMenuController";
	public const string SOUND_CONTROLLER = "SoundController";
	public const string LEVEL_CONFIG = "LevelConfig";
	public const string LEVEL_CONFIG_CONTROLLER = "LevelConfigController";

	private const string SOUND_MANAGER_PATH = "Prefabs/Managers/SoundManager";
	private const string LEVEL_MENU_MANAGER_PATH = "Prefabs/Managers/LevelMenuManager";
	private const string GAME_MANAGER_PATH = "Prefabs/Managers/GameManager";
	private const string BOARD_MANAGER_PATH = "Prefabs/Managers/BoardManager";
	private const string LEVEL_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/LevelMenuCanvas";
	private const string GAME_OVER_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/GameOverMenuCanvas";
	private const string NEXT_LEVEL_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/NextLevelMenuCanvas";


	public static BoardController BoardController
	{
		get{
			return GetPrefabInstanceController<BoardController>(BOARD_MANAGER_PATH, BOARD_CONTROLLER);
		}
	}

	public static GameController GameController
	{
		get {
			return GetPrefabInstanceController<GameController>(GAME_MANAGER_PATH, GAME_CONTROLLER);
		}
	}

	public static SoundController SoundController
	{
		get {
			return GetPrefabInstanceController<SoundController>(SOUND_MANAGER_PATH, SOUND_CONTROLLER);
		}
	}

	public static LevelMenuController LevelMenuController
	{
		get {
			return GetPrefabInstanceController<LevelMenuController>(LEVEL_MENU_MANAGER_PATH, LEVEL_MENU_CONTROLLER);
		}
	}

	public static LevelConfigController LevelConfigController
	{
		get {
			return ((LevelConfigController)(GameObject.Find (LEVEL_CONFIG).GetComponent (LEVEL_CONFIG_CONTROLLER)));
		}
	}

	public static GameObject LevelMenuCanvas
	{
		get {
			return GetPrefabInstance(LEVEL_MENU_CANVAS_PATH);
		}
	}

	public static GameObject GameOverMenuCanvas
	{
		get {
			return GetPrefabInstance(GAME_OVER_MENU_CANVAS_PATH);
		}
	}

	public static GameObject NextLevelMenuCanvas
	{
		get {
			return GetPrefabInstance(NEXT_LEVEL_MENU_CANVAS_PATH);
		}
	}


	//-------------------------------- Common ----------------------------------------


	private static string GetPrefabName (string prefabPath)
	{
		string[] path = prefabPath.Split ('/');
		string prefabName = path [path.Length - 1];
		return prefabName;
	}

	public static T GetPrefabInstanceController<T>(string prefabPath, string prefabControllerName) {
		var prefabInstance = GetPrefabInstance (prefabPath);
		return (T)Convert.ChangeType (prefabInstance.GetComponent (prefabControllerName), typeof(T));
	}

	static GameObject GetPrefabInstance (string prefabPath)
	{
		var prefabName = GetPrefabName (prefabPath);
		GameObject prefabInstance = FindByName(prefabName);
		if (!prefabInstance) {
			prefabInstance = UnityEngine.MonoBehaviour.Instantiate (Resources.Load (prefabPath)) as GameObject;
			prefabInstance.name = prefabName;
		}
		if (!prefabInstance) {
			throw new Exception (prefabName + " prefab is not found on path = " + prefabPath);
		}
		return prefabInstance;
	}

	static GameObject FindByName(string name) {
		GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (GameObject go in all) {
			if (go.name.Equals (name)) {
				return go;
			}
		}
		return null;
	}
}
