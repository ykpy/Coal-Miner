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

	public GameObject block;

	public InputField stageName;
	public Text stageSize;

	void Awake() {
		InitializeStage(stageX, stageY, stageZ);
		ShowStageSize();
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

	public bool AddBlock(uint x, uint y, uint z, Block blockType) {
		if (blockType == Block.Empty)
			return false;

		stage[x, y, z] = blockType;
		Instantiate(block, new Vector3(x, y, z), Quaternion.identity);
		return true;
	}

	public bool AddBlock(StageIndex index, Block blockType) {
		return AddBlock(index.x, index.y, index.z, blockType);
	}

	public void SaveStage() {
		string buffer = "";

		buffer += stageName.text + "\n\n";

		buffer += stageX + " " + stageY + " " + stageZ + "\n\n";

		for (uint j = 0; j < stageY; j++) {
			for (uint k = 0; k < stageZ; k++) {
				for (uint i = 0; i < stageX; i++) {
					buffer += stage[i, j, k].ToInt() + " ";
				}
				buffer += "\n";
			}
			buffer += "\n";
		}

		using (var writer = new StreamWriter(Application.dataPath + @"/../stage.dat")) {
			writer.Write(buffer);
		}
	}

	public void LoadStage() {
		stage = new Stage(LoadStageFile(Application.dataPath + @"/../stage.dat"));
		stageName.text = stage.StageName;

		stageX = stage.X;
		stageY = stage.Y;
		stageZ = stage.Z;

		ShowStageSize();

		InitializeStage(stage);
	}

	string LoadStageFile(string filePath) {
		string buffer = "";
		using (var reader = new StreamReader(filePath)) {
			buffer += reader.ReadToEnd();
		}
		return buffer;
	}

	void ShowStageSize() {
		stageSize.text = MessageUtils.FormatXYZ(stageX, stageY, stageZ);
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
}

