using System;
using UnityEngine;
using UnityEngine.UI;

public class StageEditUIManager : BaseStageUIManager {

	#region UI input
	public InputField stageName;

	public InputField stageXInput;
	public InputField stageYInput;
	public InputField stageZInput;

	public InputField createInput;
	public InputField breakInput;

	public InputField timeLimitInput;
	#endregion

	public override void ShowStageInformation(Stage stage) {
		stageName.text = stage.StageName;
		stageXInput.text = stage.X.ToString();
		stageYInput.text = stage.Y.ToString();
		stageZInput.text = stage.Z.ToString();

		createInput.text = stage.createTime.ToString();
		breakInput.text = stage.breakTime.ToString();

		timeLimitInput.text = stage.timeLimit.ToString();
	}

	public StageIndex GetStageSize() {
		return new StageIndex() {
			x = uint.Parse(stageXInput.text),
			y = uint.Parse(stageYInput.text),
			z = uint.Parse(stageZInput.text)
		};
	}

	public Stage GetStageData(Stage stage) {
		stage.StageName = stageName.text;
		stage.timeLimit = int.Parse(timeLimitInput.text);
		stage.createTime = uint.Parse(createInput.text);
		stage.breakTime = uint.Parse(breakInput.text);

		return stage;
	}

	/// <summary>
	/// 入力項目に不正な値が入力されていないかチェックします
	/// </summary>
	/// <returns>全て正常の場合、true</returns>
	public bool ValidateInputField() {

		// ステージ名が入力されていない場合、エラーメッセージを表示し、保存処理を終了する
		if (string.IsNullOrEmpty(stageName.text)) {
			ShowMessage(MessageUtils.StageNameNullErrorMessage);
			return false;
		}

		if (string.IsNullOrEmpty(stageXInput.text)) {
			return false;
		}
		if (string.IsNullOrEmpty(stageYInput.text)) {
			return false;
		}
		if (string.IsNullOrEmpty(stageZInput.text)) {
			return false;
		}

		if (string.IsNullOrEmpty(createInput.text)) {
			return false;
		}
		if (string.IsNullOrEmpty(breakInput.text)) {
			return false;
		}

		return true;
	}
}
