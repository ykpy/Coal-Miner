﻿using UnityEngine;
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
	public InputField fileName;

	public InputField stageXInput;
	public InputField stageYInput;
	public InputField stageZInput;

	public FileExplorer explorer;

	void Awake() {
		InitializeStage(stageX, stageY, stageZ);
		ShowStageSize();
	}

	void Start() {
		InitializeWall();
	}

	void Update() {
		fileName.text = fileName.text.AddFileExtension(".dat");
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
		return true;
	}

	public bool AddBlock(StageIndex index, Block blockType) {
		return AddBlock(index.x, index.y, index.z, blockType);
	}

	public bool EraseBlock(StageIndex index) {
		stage[index.x, index.y, index.z] = Block.Empty;
		return true;
	}

	public void SaveStage() {
		if (string.IsNullOrEmpty(fileName.text))
			return;

		SaveStage(fileName.text);
	}

	public void SaveStage(string fileName) {
		string buffer = "";

		if (string.IsNullOrEmpty(stageName.text)) {
			return;
		}

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

		using (var writer = new StreamWriter(Application.dataPath + @"/../StageData/" + fileName)) {
			writer.Write(buffer);
		}
	}

	public void LoadStage() {
		if (!explorer)
			explorer = FindObjectOfType<FileExplorer>();

		explorer.OpenDirectory(Application.dataPath + @"/../StageData/");
	}

	public void LoadStage(string fileName) {
		stage = new Stage(LoadStageFile(Application.dataPath + @"/../StageData/" + fileName));
		stageName.text = stage.StageName;
		this.fileName.text = fileName;

		stageX = stage.X;
		stageY = stage.Y;
		stageZ = stage.Z;

		ShowStageSize();

		InitializeStage(stage);
		InitializeWall();
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

