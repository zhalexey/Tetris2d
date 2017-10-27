using System;
using UnityEngine;


public class MoveRightAction : FigureAction
{
	public override void performAction() {
		Vector2 pos = figure.transform.position;
		pos.x += BoardController.BRICK_SIZE;
		figure.transform.position = pos;
	}
	
}

