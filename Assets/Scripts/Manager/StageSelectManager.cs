using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour {

	public static string SelectedStageFileName;

	public void SelectStage() {
		SelectedStageFileName = "tutorial.dat";
	}

	public void StartGame() {
		SceneManager.LoadScene("main");
	}
}
