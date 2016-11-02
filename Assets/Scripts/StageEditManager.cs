using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class StageEditManager : SingletonMonoBehaviour<StageEditManager> {

	bool edited = false;

	StageEditUIManager uiManager;

	void Start() {
		uiManager = FindObjectOfType<StageEditUIManager>();
	}

	#region stage edit
	/// <summary>
	/// ステージサイズを変更します
	/// </summary>
	public void ResizeStage() {
		StageIndex size = uiManager.GetStageSize();

		Stage resizedStage = new Stage(size.x, size.y, size.z, StageManager.Instance.Stage);
		StageManager.Instance.InitializeStage(resizedStage);
		StageManager.Instance.InitializeWall();
	}

	public void PlayTestGame() {
		if (!Directory.Exists(Stage.StageDataDirectoryPath + "tmp")) {
			Directory.CreateDirectory(Stage.StageDataDirectoryPath + "tmp");
		}
		string tempStageData = @"tmp/test.dat";
		SaveStage(tempStageData, uiManager.GetStageData(StageManager.Instance.Stage));
		StageSelectManager.SelectedStageFileName = tempStageData;
		SceneManager.LoadScene("main");
	}

	public void SwitchCamera() {
		CameraSwitcher.Instance.SwitchNextCamera();
		StageManager.Instance.SetPlayerToStartPosition();
	}

	#endregion


	#region file load save
	/// <summary>
	/// ステージデータファイルを読み込むファイルダイアログを表示します
	/// </summary>
	public void OpenLoadFileDialog() {
		if (edited) {
			StageUIManager.Instance.ShowMessage(MessageUtils.StageDataNotSavedWarningMessage);
		}

		var fileDialog = new System.Windows.Forms.OpenFileDialog();
		fileDialog.InitialDirectory = Stage.StageDataDirectoryPath;
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
			StageManager.Instance.LoadStage(RemoveFilePath(fileDialog.FileName));
		}
	}

	/// <summary>
	/// ステージデータを保存するファイルダイアログを表示します
	/// </summary>
	public void OpenSaveDialog() {
		if (!uiManager.ValidateInputField()) {
			return;
		}

		if (!Directory.Exists(Stage.StageDataDirectoryPath)) {
			Directory.CreateDirectory(Stage.StageDataDirectoryPath);
		}

		StageManager.Instance.RefreshScene();

		var fileDialog = new System.Windows.Forms.SaveFileDialog();
		fileDialog.InitialDirectory = Stage.StageDataDirectoryPath;
		fileDialog.CheckFileExists = false;
		fileDialog.Filter = "Stage Data (*.dat)|*.dat";
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
			var stage = StageManager.Instance.Stage;
			SaveStage(RemoveFilePath(fileDialog.FileName), uiManager.GetStageData(stage));
		}
	}

	/// <summary>
	/// ステージデータファイルパスから、ディレクトリパスを取り除き、ファイル名を抜き出します
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	/// <returns>ファイル名</returns>
	public static string RemoveFilePath(string filePath) {
		return filePath.Split(new[] { Stage.StageDataDirectoryPath }, System.StringSplitOptions.RemoveEmptyEntries)[0];
	}

	/// <summary>
	/// ステージデータをファイルに書き出します
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public void SaveStage(string fileName, Stage stage) {
		string buffer = "";

		// ステージデータを文字列に変換する

		// ステージ名
		buffer += GetCommentString(StageDataTag.StageName);
		buffer += stage.StageName + GetNewLines(2);

		// 制限時間
		buffer += GetCommentString(StageDataTag.TimeLimit);
		buffer += stage.timeLimit + GetNewLines(2);

		// ブロックを生成・破壊できる回数
		buffer += GetCommentString(StageDataTag.BlockCreateBreakNum);
		buffer += stage.createTime + " " + stage.breakTime + GetNewLines(2);
		
		// ステージサイズ
		buffer += GetCommentString(StageDataTag.StageSize);
		buffer += stage.X + " " + stage.Y + " " + stage.Z + GetNewLines(2);

		// ステージ配列
		buffer += GetCommentString(StageDataTag.Stage);
		buffer += GetCommentString("==========", 2);
		for (uint j = 0; j < stage.Y; j++) {
			for (uint k = 0; k < stage.Z; k++) {
				for (uint i = 0; i < stage.X; i++) {
					buffer += stage[i, j, k].ToInt() + " ";
				}
				buffer += "\n";
			}
			buffer += "\n";
		}
		buffer += GetCommentString("==========");

		// ファイルへの書き込み
		using (var writer = new StreamWriter(Stage.StageDataDirectoryPath + fileName)) {
			writer.Write(buffer);
		}

		// 保存成功時のメッセージ表示
		StageEditUIManager.Instance.ShowMessage(MessageUtils.StageDataSaveSuccessMessage);
		edited = false;
	}

	string GetCommentString(string str, int newLineCount = 1) {
		return "# " + str + GetNewLines(newLineCount);
	}

	string Concat(string delimiter = " ", int newLineCount = 1, params string[] str) {
		string ret = "";
		foreach (var s in str) {
			ret += s + delimiter;
		}
		return ret + GetNewLines(newLineCount);
	}

	string GetNewLines(int newLineCount) {
		return new string('\n', newLineCount);
	}
	#endregion
}
