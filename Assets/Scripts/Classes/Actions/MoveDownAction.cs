using System;
using UnityEngine;


public class MoveDownAction : FigureAction
{
	public override void performAction() {
		Vector2 pos = figure.transform.position;
		pos.y += BoardController.BRICK_SIZE;
		figure.transform.position = pos;
	}
	
}

