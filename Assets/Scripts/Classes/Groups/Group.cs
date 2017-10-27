using System.Collections.Generic;
using UnityEngine;

public class Group
{
	private List<GameObject> values = new List<GameObject> ();

	public Group (List<GameObject> values)
	{
		this.values = values;
	}

	public List<GameObject> Values {
		get { return values; }
		set{ this.values = value; }
	}

}


