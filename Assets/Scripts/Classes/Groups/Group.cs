using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Group
{
	private List<GameObject> group;
	private bool hitGroup;

	public Group() {
		hitGroup = false;
		group = new List<GameObject> ();
	}

	public void Add(GameObject item) {
		group.Add (item);
	}

	public List<GameObject> List() {
		return group;
	}

	public bool IsEmpty() {
		return group.Count == 0;
	}

	public int Count() {
		return group.Count;
	}

	public void SetHitGroup() {
		hitGroup = true;
	}

	public bool IsHitGroup() {
		return hitGroup;
	}
}


