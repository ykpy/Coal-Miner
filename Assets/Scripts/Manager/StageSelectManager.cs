using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageSelectManager : MonoBehaviour {

	public static string SelectedStageFileName;
	public static Stage SelectedStage;

	StageRepository stageRepository;

	int offset = 0;
	const int LIMIT = 1;

	List<Stage> currentStages;

	public Button nextButton;
	public Button prevButton;

	public List<Image> images;

	public Text name;
	public Text timeLimit;
	public Text createTime;
	public Text breakTime;
	public Text coin;

	public Texture2D stageImageTexture;

	void Awake() {
		stageRepository = new StageRepository();
		Cursor.visible = true;
	}

	void Start() {
		LoadStages();
		SelectStage(0);
	}

	public void SelectStage(int index) {
		try {
			SelectedStage = currentStages[index];
			ShowDescription(SelectedStage);
		} catch (System.IndexOutOfRangeException) {
			ShowDescription(null);
		} catch (System.ArgumentOutOfRangeException) {
			ShowDescription(null);
		}
		
	}

	public void StartGame() {
		SceneManager.LoadScene("main");
	}

	public void GoToEdit() {
		SceneManager.LoadScene("stage");
	}

	public void ShowNextPage() {
		offset += LIMIT;
		LoadStages();
		SelectStage(0);
	}

	public void ShowPrevPage() {
		offset -= LIMIT;
		LoadStages();
		SelectStage(0);
	}

	void LoadStages() {
		currentStages = stageRepository.FindByLimitAndOffset(LIMIT, offset);
		ShowImages(currentStages);
		CheckPages();
	}

	void CheckPages() {
		if (stageRepository.HasNext(LIMIT, offset)) {
			nextButton.gameObject.SetActive(true);
		} else {
			nextButton.gameObject.SetActive(false);
		}
		if (stageRepository.HasPrev(LIMIT, offset - LIMIT)) {
			prevButton.gameObject.SetActive(true);
		} else {
			prevButton.gameObject.SetActive(false);
		}
	}

	void ShowImages(List<Stage> stages) {

		int i = 0;
		for (; i < stages.Count; i++) {
			images[i].material.mainTexture = null;
			images[i].sprite = Sprite.Create(stageImageTexture, new Rect(0, 0, 128, 128), Vector2.zero);
			//stages[i];
		}
		for (; i < images.Count; i++) {
			images[i].sprite = null;
		}
	}

	void ShowDescription(Stage stage) {
		if (stage != null) {
			name.text = stage.StageName;
			timeLimit.text = stage.timeLimit.ToString();
			createTime.text = stage.createTime.ToString();
			breakTime.text = stage.breakTime.ToString();
			coin.text = stage.GetCoinCount().ToString();
		} else {
			name.text = "";
			timeLimit.text = "";
			createTime.text = "";
			breakTime.text = "";
			coin.text = "";
		}
	}

	public static bool IsStageSelected() {
		return SelectedStage != null || SelectedStageFileName != null;
	}
}
