using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;


public class BoardGizmos : MonoBehaviour {

	private float DELTA = BoardController.BRICK_SIZE / 2;

	void Update () {
		DrawGrid ();
	}

	void DrawGrid() {
		for (int i = 0; i < BoardController.BOARD_HEIGHT; i++)
			for (int j = 0; j < BoardController.BOARD_WIDTH; j++) {
				Vector2 pos = ScriptManager.BoardController.getPos (j, i);
				DrawBox (pos);
			}
	}

	void DrawBox(Vector2 pos) {
		
		Vector2 pointA = new Vector2 (pos.x - DELTA, pos.y + DELTA);
		Vector2 pointB = new Vector2 (pos.x + DELTA, pos.y + DELTA);
		Debug.DrawLine (pointA, pointB, Color.yellow);

		pointA = new Vector2 (pos.x + DELTA, pos.y + DELTA);
		pointB = new Vector2 (pos.x + DELTA, pos.y - DELTA);
		Debug.DrawLine (pointA, pointB, Color.yellow);

	}

	public static void ClearLog()
	{
		var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
		var type = assembly.GetType("UnityEditorInternal.LogEntries");
		var method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}

	public static void DrawBoxAim(Vector2 pos) {
		Vector2 pointA = new Vector2 (pos.x - BoardController.BRICK_SIZE, pos.y);
		Vector2 pointB = new Vector2 (pos.x + BoardController.BRICK_SIZE, pos.y);
		Debug.DrawLine (pointA, pointB, Color.red);

		pointA = new Vector2 (pos.x, pos.y + BoardController.BRICK_SIZE);
		pointB = new Vector2 (pos.x, pos.y - BoardController.BRICK_SIZE);
		Debug.DrawLine (pointA, pointB, Color.red);
	}


}
