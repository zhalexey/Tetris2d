using System;
using UnityEngine;

public class SoundController: MonoBehaviour
{
	public enum SoundAction
	{
		Rotate,
		Falling,
		BurnLine
	}

	public AudioClip rotateSound;
	public AudioClip fallingSound;
	public AudioClip burnLineSound;


	public void PlaySound (SoundAction action)
	{

		AudioSource freeSource = null;
		AudioSource[] sources = gameObject.GetComponents<AudioSource> ();
		foreach (AudioSource source in sources) {
			if (!source.isPlaying) {
				freeSource = source;
			}
		}

		if (freeSource == null) {
			return;
		}

		switch (action) {
		case SoundAction.Rotate:
			freeSource.clip = rotateSound;
			break;
		case SoundAction.Falling:
			freeSource.clip = fallingSound;
			break;
		case SoundAction.BurnLine:
			freeSource.clip = burnLineSound;
			break;

		}
		freeSource.PlayOneShot (freeSource.clip);
	}



}
