using UnityEngine;
using System.Collections.Generic;

public class CameraSwitcher : SingletonMonoBehaviour<CameraSwitcher> {

	public List<Camera> cameras;
	int currentIndex;

	public Camera MainCamera {
		get {
			return cameras[currentIndex];
		}
	}

	protected override void Awake() {
		base.Awake();

		currentIndex = 0;

		DisableAllCameras();
		MainCamera.gameObject.SetActive(true);
	}

	public void SwitchNextCamera() {
		DisableAllCameras();
		currentIndex++;
		if (currentIndex >= cameras.Count)
			currentIndex = 0;
		cameras[currentIndex].gameObject.SetActive(true);
	}

	void DisableAllCameras() {
		foreach (var camera in cameras) {
			camera.gameObject.SetActive(false);
		}
	}

	public bool IsMain(Camera camera) {
		return MainCamera == camera;
	}
}

