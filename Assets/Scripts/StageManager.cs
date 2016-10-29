using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class StageManager : MonoBehaviour {

	static StageManager _instance;
	public static StageManager Instance {
		get {
			return _instance ?? (_instance = FindObjectOfType<StageManager>());
		}
	}

	public uint stageX;
	public uint stageY;
	public uint stageZ;

	Stage stage;

	public GameObject groundBlock;
	public GameObject unbreakableBlock;
	public GameObject breakableBlock;

	public GameObject wall;

	readonly Vector3 defaultWallScale = new Vector3(1f, 0.01f, 1f);

	public InputField stageName;

	public InputField stageXInput;
	public InputField stageYInput;
	public InputField stageZInput;

	public Text message;

	public bool edited = false;

	void Awake() {
		InitializeStage(stageX, stageY, stageZ);
		ShowStageSize();
	}

	void Start() {
		InitializeWall();
	}

	void InitializeWall() {
		wall.transform.localScale = defaultWallScale;
		wall.transform.localScale = new Vector3(stageX, wall.transform.localScale.y, stageZ);
		wall.transform.position = new Vector3(stageX - 1, wall.transform.position.y * 2, stageZ - 1) / 2;
	}

	bool InitializeStage(uint x, uint y, uint z) {
		DestroyAllBlocks();
		stage = new Stage(x, y, z);

		DoToStage((s, i, j, k) => {
			AddBlock(new StageIndex { x = i, y = j, z = k }, s[i, j, k]);
		});

		return true;
	}

	bool InitializeStage(Stage stage) {
		stageX = stage.X;
		stageY = stage.Y;
		stageZ = stage.Z;

		DestroyAllBlocks();
		this.stage = stage;
		DoToStage((s, i, j, k) => {
			AddBlock(new StageIndex { x = i, y = j, z = k }, s[i, j, k]);
		});
		return true;
	}

	void DestroyAllBlocks() {
		foreach (var block in GameObject.FindGameObjectsWithTag(Tags.BLOCK)) {
			DestroyImmediate(block);
		}
	}

	public void ResizeStage() {
		InitializeStage(new Stage(uint.Parse(stageXInput.text), uint.Parse(stageYInput.text), uint.Parse(stageZInput.text), stage));
		InitializeWall();
	}

	GameObject GetBlockObjectByBlockType(Block blockType) {
		switch (blockType) {
			case Block.Breakable:
				return breakableBlock;
			case Block.Ground:
				return groundBlock;
			case Block.Unbreakable:
				return unbreakableBlock;
			default:
				return null;
		}
	}

	public bool AddBlock(uint x, uint y, uint z, Block blockType) {
		if (blockType == Block.Empty)
			return false;

		stage[x, y, z] = blockType;
		Instantiate(GetBlockObjectByBlockType(blockType), new Vector3(x, y, z), Quaternion.identity);

		edited = true;

		return true;
	}

	public bool AddBlock(StageIndex index, Block blockType) {
		return AddBlock(index.x, index.y, index.z, blockType);
	}

	public bool EraseBlock(StageIndex index) {
		stage[index.x, index.y, index.z] = Block.Empty;

		edited = true;
		return true;
	}

	public void OpenSaveDialog() {
		// ステージ名が入力されていない場合、エラーメッセージを表示し、保存処理を終了する
		if (string.IsNullOrEmpty(stageName.text)) {
			message.text = MessageUtils.StageNameNullErrorMessage;
			return;
		}

		var fileDialog = new System.Windows.Forms.SaveFileDialog();
		fileDialog.InitialDirectory = Stage.StageDataDirectoryPath;
		fileDialog.CheckFileExists = false;
		fileDialog.Filter = "Stage Data(*.dat)|*.dat";
		if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
			SaveStage(RemoveFilePath(fileDialog.FileName));
		}
	}

	public void SaveStage(string fileName) {
		string buffer = "";

		// ステージデータを文字列に変換する
		buffer += stageName.text + "\n\n";

		buffer += stage.X + " " + stage.Y + " " + stage.Z + "\n\n";

		for (uint j = 0; j < stage.Y; j++) {
			for (uint k = 0; k < stage.Z; k++) {
				for (uint i = 0; i < stage.X; i++) {
					buffer += stage[i, j, k].ToInt() + " ";
				}
				buffer += "\n";
			}
			buffer += "\n";
		}

		using (var writer = new StreamWriter(Stage.StageDataDirectoryPath + fileName)) {
			writer.Write(buffer);
		}

		// 保存成功時のメッセージ表示
		message.text = MessageUtils.StageDataSaveSuccessMessage;
		edited = false;
	}

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

	public void LoadStage(string fileName) {
		stage = new Stage(LoadStageFile(Stage.StageDataDirectoryPath + fileName));
		stageName.text = stage.StageName;

		stageX = stage.X;
		stageY = stage.Y;
		stageZ = stage.Z;

		ShowStageSize();

		InitializeStage(stage);
		InitializeWall();

		message.text = MessageUtils.StageDataLoadSuccessMessage;
	}

	public static string RemoveFilePath(string filePath) {
		return filePath.Split(new[] { Stage.StageDataDirectoryPath }, System.StringSplitOptions.RemoveEmptyEntries)[0];
	}

	string LoadStageFile(string filePath) {
		string buffer = "";
		using (var reader = new StreamReader(filePath)) {
			buffer += reader.ReadToEnd();
		}
		return buffer;
	}

	void ShowStageSize() {
		stageXInput.text = stageX.ToString();
		stageYInput.text = stageY.ToString();
		stageZInput.text = stageZ.ToString();
	}

	delegate void ActionStage(Stage stage, uint i, uint j, uint k);

	void DoToStage(ActionStage action) {
		for (uint j = 0; j < stageY; j++) {
			for (uint k = 0; k < stageZ; k++) {
				for (uint i = 0; i < stageX; i++) {
					action(stage, i, j, k);
				}
			}
		}
	}

	public GameObject FindBlockByPosition(Vector3 position) {
		return GameObject.FindGameObjectsWithTag(Tags.BLOCK)
			.Where(block => block.transform.position == BlockUtils.RoundPosition(position))
			.First();
	}

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
}

