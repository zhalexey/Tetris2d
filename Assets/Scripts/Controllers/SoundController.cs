using System;
using UnityEngine;

/**
 *  Script is common for all scenes
 */

public class SoundController: MonoBehaviour
{
	public static bool isCreated;
	public static bool isMusicOn;
	public static bool isSoundOn;

	public enum SoundAction
	{
		Rotate,
		Falling,
		BurnLine,
		CountCoin,
		MenuBtnHover,
		MenuBtnClick,
	}


	private AudioSource mainMenuMusicAudioSource;
	private AudioSource gameMusicAudioSource;
	private AudioSource gameSoundAudioSource;

	public AudioClip mainMenuMusic;
	public AudioClip rotateSound;
	public AudioClip fallingSound;
	public AudioClip burnLineSound;
	public AudioClip countCoinSound;
	public AudioClip menuBtnHoverSound;
	public AudioClip menuBtnClickSound;

	public AudioClip calmMusic;
	public AudioClip energyMusic;
	public AudioClip mapMusic;



	/**
	 * Prevent destroying for further usage in other scenes
	 */
	void Awake() {
		if (!isCreated) {
			DontDestroyOnLoad (transform.gameObject);	
			isCreated = true;
			isMusicOn = true;
			isSoundOn = true;
		} else {
			Destroy (this.gameObject);
		}

		InitSources ();
	}


	void InitSources() {
		mainMenuMusicAudioSource = gameObject.AddComponent<AudioSource>();
		mainMenuMusicAudioSource.clip = mainMenuMusic;
		mainMenuMusicAudioSource.loop = true;

		gameMusicAudioSource = gameObject.AddComponent<AudioSource>();
		gameSoundAudioSource = gameObject.AddComponent<AudioSource>();
	}


	//------------------------------------------------------

	public void PlaySound (SoundAction action)
	{
		if (!isSoundOn) {
			return;
		}

		switch (action) {
		case SoundAction.Rotate:
			gameSoundAudioSource.clip = rotateSound;
			break;
		case SoundAction.Falling:
			gameSoundAudioSource.clip = fallingSound;
			break;
		case SoundAction.BurnLine:
			gameSoundAudioSource.clip = burnLineSound;
			break;
		case SoundAction.CountCoin:
			gameSoundAudioSource.clip = countCoinSound;
			break;
		case SoundAction.MenuBtnHover:
			gameSoundAudioSource.clip = menuBtnHoverSound;
			break;
		case SoundAction.MenuBtnClick:
			gameSoundAudioSource.clip = menuBtnClickSound;
			break;
		}
		gameSoundAudioSource.PlayOneShot (gameSoundAudioSource.clip);
	}


	public void PlayBtnHoverSound() {
		PlaySound (SoundAction.MenuBtnHover);
	}


	//--------------------------------------- Music ------------------------------

	public void PlayCalmMusic() {
		gameMusicAudioSource.Stop ();
		gameMusicAudioSource.loop = true;
		gameMusicAudioSource.clip = calmMusic;

		if (isMusicOn) {
			gameMusicAudioSource.Play ();
		}
	}


	public void PlayEnergyMusic() {
		gameMusicAudioSource.Stop ();
		gameMusicAudioSource.loop = true;
		gameMusicAudioSource.clip = energyMusic;

		if (isMusicOn) {
			gameMusicAudioSource.Play ();
		}
	}

	public void PlayMapMusic() {
		gameMusicAudioSource.Stop ();
		gameMusicAudioSource.loop = true;
		gameMusicAudioSource.clip = mapMusic;

		if (isMusicOn) {
			gameMusicAudioSource.Play ();
		}
	}


	public void PlayMenuTheme ()
	{
		if (isMusicOn && !mainMenuMusicAudioSource.isPlaying) {
			mainMenuMusicAudioSource.Play ();
		}
	}


	public void PauseMenuTheme ()
	{
		mainMenuMusicAudioSource.Pause ();
	}


	public void PauseGameTheme() {
		gameMusicAudioSource.Pause ();
	}


	public void PlayGameTheme() {
		if (isMusicOn && !gameMusicAudioSource.isPlaying) {
			gameMusicAudioSource.Play ();
		}
	}


	public void MusicOff() {
		PlaySound (SoundAction.MenuBtnClick);
		isMusicOn = false;
		PauseMenuTheme ();
		PauseGameTheme ();
	}


	public void MusicOn() {
		PlaySound (SoundAction.MenuBtnClick);
		isMusicOn = true;
		PlayMenuTheme ();
	}

	public void SoundOff() {
		isSoundOn = false;
	}

	public void SoundOn() {
		isSoundOn = true;
	}



}
