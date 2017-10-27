using System;
using UnityEngine;


public abstract class FigureAction
{
	protected GameObject figure;

	public void setObject (GameObject figure) {
		this.figure = figure;
	}

	abstract public void performAction();
}

