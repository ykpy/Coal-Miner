using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : SingletonMonoBehaviour<TitleManager> {

	void Update () {
		if (Input.anyKeyDown) {
			SceneManager.LoadScene("select");
		}
	}
}
