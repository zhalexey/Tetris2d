using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelMenuController : MonoBehaviour
{
	public GameObject canvas;

	public void ActivateMenu() {
		canvas.SetActive (true);
	}

	public void DeactivateMenu ()
	{
		canvas.SetActive (false);
	}

	public void OnBackToMainMenuClick ()
	{
		SceneManager.LoadScene ("MainMenu");
	}

}
