using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	AudioSource audioSource;
	AudioSource soundEffectSource;

	public List<AudioClip> soundEffects;

	public List<AudioClip> bgm;

	public bool Loop {
		get { return audioSource.loop; }
		set { audioSource.loop = value; }
	}

	protected override void Awake() {
		base.Awake();
		audioSource = GetComponent<AudioSource>();
		var audioSources = GetComponents<AudioSource>();
		soundEffectSource = audioSources[1];
	}

	public void PlayBGM(int index) {
		audioSource.Stop();
		audioSource.clip = bgm[index];
		audioSource.Play();
	}

	public void PlaySoundEffect(int index) {
		soundEffectSource.PlayOneShot(soundEffects[index]);
	}
}
