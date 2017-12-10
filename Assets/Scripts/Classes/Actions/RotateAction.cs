using System;
using UnityEngine;


public class RotateAction : FigureAction
{
	public override void performAction() {
		figure.transform.rotation *= Quaternion.AngleAxis (-90, Vector3.forward) ;
	}
	
}

