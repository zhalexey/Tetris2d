using System;
using UnityEngine;


public class MoveLeftAction : FigureAction
{
	public override void performAction() {
		Vector2 pos = figure.transform.position;
		pos.x -= BoardController.BRICK_SIZE;
		figure.transform.position = pos;
	}
	
}

