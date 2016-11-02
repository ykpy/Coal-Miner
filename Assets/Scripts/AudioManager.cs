using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	AudioSource audioSource;

	public List<AudioClip> soundEffects;

	public List<AudioClip> bgm;

	protected override void Awake() {
		base.Awake();
		audioSource = GetComponent<AudioSource>();
	}

	public void PlayBGM(int index) {
		audioSource.Stop();
		audioSource.clip = bgm[index];
		audioSource.Play();
	}

	public void PlaySoundEffect(int index) {
		audioSource.PlayOneShot(soundEffects[index]);
	}
}
