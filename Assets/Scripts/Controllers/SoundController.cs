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

	public AudioClip calmMusic;
	public AudioClip energyMusic;

	public bool musicOff = true;


	public void PlaySound (SoundAction action)
	{

		AudioSource freeSource = GetFreeAudioSource ();

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


	private AudioSource GetFreeAudioSource ()
	{
		AudioSource[] sources = gameObject.GetComponents<AudioSource> ();
		foreach (AudioSource source in sources) {
			if (!source.isPlaying) {
				return (source);
			}
		}
		return null;
	}


	public void PlayCalmMusic() {
		if (musicOff) {
			return;
		}
		AudioSource musicAudioSource = ScriptManager.MusicAudioSource;
		musicAudioSource.Stop ();
		musicAudioSource.loop = true;
		musicAudioSource.clip = calmMusic;
		musicAudioSource.Play ();
	}

	public void PlayEnergyMusic() {
		if (musicOff) {
			return;
		}
		AudioSource musicAudioSource = ScriptManager.MusicAudioSource;
		musicAudioSource.Stop ();
		musicAudioSource.loop = true;
		musicAudioSource.clip = energyMusic;
		musicAudioSource.Play ();
	}

}
