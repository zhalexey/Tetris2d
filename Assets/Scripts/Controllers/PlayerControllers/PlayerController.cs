using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static int levelNumber = 1;
	public static int levelMaxAchieved = 1;


	public static void ResetLevel() {
		levelNumber = 1;
	}

	public static void NextLevel() {
		levelNumber++;
		levelMaxAchieved++;
	}

	public static bool HasAchievedLevel(int level) {
		return levelNumber == level;
	}

}
