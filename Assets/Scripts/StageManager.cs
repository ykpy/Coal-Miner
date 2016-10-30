using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class StageManager : SingletonMonoBehaviour<StageManager> {

	Stage stage;
	GameObject[,,] stageBlockObjects;
	Transform startTransform;
	public Transform stageTransform;

	public GameObject player;

	#region block
	public GameObject groundBlock;
	public GameObject unbreakableBlock;
	public GameObject breakableBlock;
	public GameObject startBlock;
	public GameObject goalBlock;
	#endregion

	public GameObject wall;
	readonly Vector3 defaultWallScale = new Vector3(1f, 0.01f, 1f);

	#region UI input
	public InputField stageName;

	public InputField stageXInput;
	public InputField stageYInput;
	public InputField stageZInput;

	public InputField createInput;
	public InputField breakInput;

	public InputField timeLimitInput;
	#endregion

	public Text message;

	bool edited = false;

	protected override void Awake() {
		base.Awake();

		stageXInput.text = stageYInput.text = stageZInput.text = "10";
		InitializeStage(uint.Parse(stageXInput.text), uint.Parse(stageYInput.text), uint.Parse(stageZInput.text));
		ShowStageSize();

		createInput.text = "0";
		breakInput.text = "0";

		timeLimitInput.text = "60";
	}

	void Start() {
		InitializeWall();

		SetPlayerToStartPosition();
	}

	/// <summary>
	/// 底面サイズを初期化します
	/// </summary>
	void InitializeWall() {
		wall.transform.localScale = defaultWallScale;
		wall.transform.localScale = new Vector3(stage.X, wall.transform.localScale.y, stage.Z);
		wall.transform.position = new Vector3(stage.X - 1, wall.transform.position.y * 2, stage.Z - 1) / 2;
	}

	/// <summary>
	/// ステージ全体を初期化します
	/// </summary>
	/// <param name="x">幅</param>
	/// <param name="y">高さ</param>
	/// <param name="z">奥行き</param>
	/// <returns></returns>
	bool InitializeStage(uint x, uint y, uint z) {
		DestroyAllBlocks();
		stage = new Stage(x, y, z);
		stageBlockObjects = new GameObject[x, y, z];

		DoToStage((s, i, j, k) => {
			AddBlock(new StageIndex { x = i, y = j, z = k }, s[i, j, k]);
		});

		return true;
	}

	/// <summary>
	/// ステージサイズを変更します
	/// </summary>
	/// <param name="stage">元となるステージ</param>
	/// <returns></returns>
	bool InitializeStage(Stage stage) {
		DestroyAllBlocks();
		this.stage = stage;
		stageBlockObjects = new GameObject[stage.X, stage.Y, stage.Z];

		DoToStage((s, i, j, k) => {
			AddBlock(new StageIndex { x = i, y = j, z = k }, s[i, j, k]);
		});

		return true;
	}

	/// <summary>
	/// 全てのブロックを破棄します
	/// </summary>
	void DestroyAllBlocks() {
		foreach (var block in GameObject.FindGameObjectsWithTag(Tags.BLOCK)) {
			DestroyImmediate(block);
		}
	}

	/// <summary>
	/// ステージサイズを変更します
	/// </summary>
	public void ResizeStage() {
		InitializeStage(new Stage(uint.Parse(stageXInput.text), uint.Parse(stageYInput.text), uint.Parse(stageZInput.text), stage));
		InitializeWall();
	}

	/// <summary>
	/// ブロックタイプからブロックオブジェクトを取得します
	/// </summary>
	/// <param name="blockType">ブロックタイプ</param>
	/// <returns>タイプに対応するブロックゲームオブジェクト</returns>
	GameObject GetBlockObjectByBlockType(Block blockType) {
		switch (blockType) {
			case Block.Breakable:
				return breakableBlock;
			case Block.Ground:
				return groundBlock;
			case Block.Unbreakable:
				return unbreakableBlock;
			case Block.Start:
				return startBlock;
			case Block.Goal:
				return goalBlock;
			default:
				throw new System.Exception();
		}
	}

	/// <summary>
	/// ステージにブロックを追加します
	/// </summary>
	/// <param name="x">X座標</param>
	/// <param name="y">Y座標</param>
	/// <param name="z">Z座標</param>
	/// <param name="blockType">ブロックタイプ</param>
	/// <returns>追加に成功した場合、true</returns>
	public bool AddBlock(uint x, uint y, uint z, Block blockType) {
		if (blockType == Block.Empty)
			return false;

		stage[x, y, z] = blockType;
		var obj = Instantiate(GetBlockObjectByBlockType(blockType), new Vector3(x, y, z), Quaternion.identity) as GameObject;
		obj.GetComponent<BlockStatus>().stageIndex = new StageIndex() { x = x, y = y, z = z };
		obj.transform.SetParent(stageTransform);

		stageBlockObjects[x, y, z] = obj;

		if (blockType == Block.Start) {
			startTransform = obj.transform;
		}

		edited = true;

		return true;
	}

	/// <summary>
	/// ブロックを追加します
	/// </summary>
	/// <param name="index"></param>
	/// <param name="blockType"></param>
	/// <returns></returns>
	public bool AddBlock(StageIndex index, Block blockType) {
		return AddBlock(index.x, index.y, index.z, blockType);
	}

	/// <summary>
	/// ステージからブロックを消去します
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public bool EraseBlock(StageIndex index) {
		stage[index.x, index.y, index.z] = Block.Empty;
		Destroy(stageBlockObjects[index.x, index.y, index.z]);
		edited = true;
		return true;
	}

	/// <summary>
	/// ステージデータを保存するファイルダイアログを表示します
	/// </summary>
	public void OpenSaveDialog() {
		// ステージ名が入力されていない場合、エラーメッセージを表示し、保存処理を終了する
		if (string.IsNullOrEmpty(stageName.text)) {
			message.text = MessageUtils.StageNameNullErrorMessage;
			return;
		}

		if (!ValidateInputField()) {
			return;
		}

		var fileDialog = new System.Windows.Forms.SaveFileDialog();
		fileDialog.InitialDirectory = Stage.StageDataDirectoryPath;
		fileDialog.CheckFileExists = false;
		fileDialog.Filter = "Stage Data (*.dat)|*.dat";
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
			SaveStage(RemoveFilePath(fileDialog.FileName));
		}
	}

	/// <summary>
	/// ステージデータをファイルに書き出します
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public void SaveStage(string fileName) {
		string buffer = "";

		// ステージデータを文字列に変換する

		// ステージ名
		buffer += GetCommentString(StageDataTag.StageName);
		buffer += stageName.text + "\n\n";

		// 制限時間
		buffer += GetCommentString(StageDataTag.TimeLimit);
		buffer += timeLimitInput.text + "\n\n";

		// ブロックを生成・破壊できる回数
		buffer += GetCommentString(StageDataTag.BlockCreateBreakNum);
		buffer += createInput.text + " " + breakInput.text + "\n\n";

		// ステージサイズ
		buffer += GetCommentString(StageDataTag.StageSize);
		buffer += stage.X + " " + stage.Y + " " + stage.Z + "\n\n";

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
		message.text = MessageUtils.StageDataSaveSuccessMessage;
		edited = false;
	}

	string GetCommentString(string str, int newLineCount = 1) {
		str = "# " + str;
		for (int i = 0; i < newLineCount; i++) {
			str += "\n";
		}
		return str;
	}

	/// <summary>
	/// ステージデータファイルを読み込むファイルダイアログを表示します
	/// </summary>
	public void OpenLoadFileDialog() {
		if (edited) {
			message.text = MessageUtils.StageDataNotSavedWarningMessage;
		}

		var fileDialog = new System.Windows.Forms.OpenFileDialog();
		fileDialog.InitialDirectory = Stage.StageDataDirectoryPath;
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
			LoadStage(RemoveFilePath(fileDialog.FileName));
		}
	}

	/// <summary>
	/// ステージデータファイルを読み込みます
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public void LoadStage(string fileName) {
		stage = new Stage(LoadStageFile(Stage.StageDataDirectoryPath + fileName));
		stageName.text = stage.StageName;

		ShowStageSize();

		createInput.text = stage.createTime.ToString();
		breakInput.text = stage.breakTime.ToString();
		timeLimitInput.text = stage.timeLimit.ToString();

		InitializeStage(stage);
		InitializeWall();

		message.text = MessageUtils.StageDataLoadSuccessMessage;

		SetPlayerToStartPosition();
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
	/// ステージデータファイルを文字列として読み込みます
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	/// <returns>ステージデータファイルの内容</returns>
	string LoadStageFile(string filePath) {
		string buffer = "";
		using (var reader = new StreamReader(filePath)) {
			buffer += reader.ReadToEnd();
		}
		return buffer;
	}

	/// <summary>
	/// ステージサイズを画面に表示します
	/// </summary>
	void ShowStageSize() {
		stageXInput.text = stage.X.ToString();
		stageYInput.text = stage.Y.ToString();
		stageZInput.text = stage.Z.ToString();
	}


	delegate void ActionStage(Stage stage, uint i, uint j, uint k);
	/// <summary>
	/// ステージ配列に対して処理を行います
	/// </summary>
	/// <param name="action">処理</param>
	void DoToStage(ActionStage action) {
		for (uint j = 0; j < stage.Y; j++) {
			for (uint k = 0; k < stage.Z; k++) {
				for (uint i = 0; i < stage.X; i++) {
					action(stage, i, j, k);
				}
			}
		}
	}

	/// <summary>
	/// ワールド座標から、ブロックオブジェクトを取得します
	/// </summary>
	/// <param name="position">ワールド座標</param>
	/// <returns>指定のワールド座標に一致するブロックオブジェクト</returns>
	public GameObject FindBlockByPosition(Vector3 position) {
		return GameObject.FindGameObjectsWithTag(Tags.BLOCK)
			.Where(block => block.transform.position == BlockUtils.RoundPosition(position))
			.First();
	}

	/// <summary>
	/// ブロックタイプからテクスチャを取得します
	/// </summary>
	/// <param name="blockType">ブロックタイプ</param>
	/// <returns>一致するテクスチャ</returns>
	public Texture GetTexture(Block blockType) {
		switch (blockType) {
			case Block.Breakable:
				return BlockUtils.GetTextureFromMaterial(breakableBlock.GetMaterial());
			case Block.Unbreakable:
				return BlockUtils.GetTextureFromMaterial(unbreakableBlock.GetMaterial());
			case Block.Ground:
				return BlockUtils.GetTextureFromMaterial(groundBlock.GetMaterial());
			default:
				return null;
		}
	}

	/// <summary>
	/// 入力項目に不正な値が入力されていないかチェックします
	/// </summary>
	/// <returns>全て正常の場合、true</returns>
	bool ValidateInputField() {

		if (string.IsNullOrEmpty(stageName.text)) {
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

	void SetPlayerToStartPosition() {
		if (startTransform)
			player.transform.position = startTransform.position;
	}
}

