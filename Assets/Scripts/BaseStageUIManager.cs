using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class BaseStageUIManager : SingletonMonoBehaviour<BaseStageUIManager> {

	[SerializeField]
	Text messageText;

	public abstract void ShowStageInformation(Stage stage);

	public void ShowMessage(string message) {
		messageText.text = message;
	}
}
