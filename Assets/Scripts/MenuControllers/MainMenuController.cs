using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{


	public void onStartClick ()
	{
		SceneManager.LoadScene ("Level1");
	}

	public void onQuitClick ()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit ();
		#endif
	}



}
