using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptManager {

	public const string BOARD_CONTROLLER = "BoardController";
	public const string GAME_CONTROLLER = "GameController";
	public const string LEVEL_MENU_CONTROLLER = "LevelMenuController";
	public const string SOUND_CONTROLLER = "SoundController";
	public const string LEVEL_CONFIG = "LevelConfig";
	public const string LEVEL_CONFIG_CONTROLLER = "LevelConfigController";
	public const string CAMERA = "Camera";
	public const string CAMERA_CONTROLLER = "CameraController";

	private const string SOUND_MANAGER_PATH = "Prefabs/Managers/SoundManager";
	private const string LEVEL_MENU_MANAGER_PATH = "Prefabs/Managers/LevelMenuManager";
	private const string GAME_MANAGER_PATH = "Prefabs/Managers/GameManager";
	private const string BOARD_MANAGER_PATH = "Prefabs/Managers/BoardManager";
	private const string LEVEL_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/LevelMenuCanvas";
	private const string GAME_OVER_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/GameOverMenuCanvas";
	private const string NEXT_LEVEL_MENU_CANVAS_PATH = "Prefabs/Canvases/MenuCanvases/NextLevelMenuCanvas";
	private const string BACKGROUND_CANVAS_PATH = "Prefabs/Canvases/CameraCanvases/BackgroundCanvas";
	private const string TIMESCALE_CANVAS_PATH = "Prefabs/Canvases/CameraCanvases/TimeScaleCanvas";
	private const string TREASUREBOX_CANVAS_PATH = "Prefabs/Canvases/CameraCanvases/TreasureBoxCanvas";


	private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

	static ScriptManager() {
		SceneManager.activeSceneChanged += ResetCache;
	}

	private static void ResetCache(Scene prev, Scene next) {
		cache = new Dictionary<string, GameObject> ();
	}


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
			return GetPrefabInstanceCached(LEVEL_MENU_CANVAS_PATH);
		}
	}

	public static GameObject GameOverMenuCanvas
	{
		get {
			return GetPrefabInstanceCached(GAME_OVER_MENU_CANVAS_PATH);
		}
	}

	public static GameObject NextLevelMenuCanvas
	{
		get {
			return GetPrefabInstanceCached(NEXT_LEVEL_MENU_CANVAS_PATH);
		}
	}

	public static GameObject BackgroundCanvas
	{
		get {
			return GetPrefabInstance (BACKGROUND_CANVAS_PATH);
		}
	}

	public static GameObject TimeScaleCanvas
	{
		get {
			return GetPrefabInstance (TIMESCALE_CANVAS_PATH);
		}
	}

	public static GameObject TreasureBoxCanvas
	{
		get {
			return GetPrefabInstance (TREASUREBOX_CANVAS_PATH);
		}
	}


	public static GameObject Camera
	{
		get {
			return GetPrefabInstance (CAMERA);
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

	static GameObject GetPrefabInstance (string prefabPath, bool isCached)
	{
		var prefabName = GetPrefabName (prefabPath);

		GameObject prefabInstance = null;

		if (isCached) {
			cache.TryGetValue (prefabName, out prefabInstance);
			if (prefabInstance != null) {
				return prefabInstance;
			}
		}

		prefabInstance = GameObject.Find(prefabName);

		if (!prefabInstance) {
			prefabInstance = UnityEngine.MonoBehaviour.Instantiate (Resources.Load (prefabPath)) as GameObject;
			prefabInstance.name = prefabName;

			if (isCached && !cache.ContainsKey(prefabName)) {
				cache.Add (prefabName, prefabInstance);
			}
		}
		if (!prefabInstance) {
			throw new Exception (prefabName + " prefab is not found on path = " + prefabPath);
		}
		return prefabInstance;
	}

	static GameObject GetPrefabInstanceCached (string prefabPath)
	{
		return GetPrefabInstance (prefabPath, true);
	}

	static GameObject GetPrefabInstance (string prefabPath)
	{
		return GetPrefabInstance (prefabPath, false);
	}

//	static GameObject FindByName(string name) {
//		List<Transform> all = FindObjectsOfTypeAll<Transform>();
//		foreach (Transform tr in all) {
//			if (tr.name.Equals (name)) {
//				return tr.gameObject;
//			}
//		}
//		return null;
//	}

}
