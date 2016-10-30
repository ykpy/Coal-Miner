using UnityEngine;
using System.IO;
using System.Linq;


public class StageManager : SingletonMonoBehaviour<StageManager> {

	Stage stage;
	public Stage Stage {
		get { return stage; }
	}
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

	Game game;
	StageUIManager UIManager;

	protected override void Awake() {
		base.Awake();

		InitializeStage(10, 10, 10);
		BaseStageUIManager.Instance.ShowStageInformation(stage);
	}

	void Start() {
		UIManager = FindObjectOfType<StageUIManager>();

		InitializeWall();

		SetPlayerToStartPosition();

		if (StageSelectManager.SelectedStageFileName != null) {
			LoadStage(StageSelectManager.SelectedStageFileName);
			game = new Game(stage);
			UIManager.ShowGameInformation(game);
		}
	}

	void Update() {
		if (game != null && game.isStarted) {
			if (!game.IsTimeOver) {
				game.limitTime -= Time.deltaTime;
			}
			UIManager.ShowTimeLimit(Mathf.CeilToInt(game.limitTime));
		} else {
			if (Input.GetKeyDown(KeyCode.Return)) {
				StartGame();
			}
		}
	}

	public void StartGame() {
		game.StartGame();
		UIManager.ShowGameInformation(game);
	}

	/// <summary>
	/// 底面サイズを初期化します
	/// </summary>
	public void InitializeWall() {
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
	public bool InitializeStage(uint x, uint y, uint z) {
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
	public bool InitializeStage(Stage stage) {
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
		return true;
	}

	/// <summary>
	/// ステージデータファイルを読み込みます
	/// </summary>
	/// <param name="fileName">ファイル名</param>
	public void LoadStage(string fileName) {
		stage = new Stage(LoadStageFile(Stage.StageDataDirectoryPath + fileName));

		// ステージ情報の表示
		BaseStageUIManager.Instance.ShowStageInformation(stage);

		InitializeStage(stage);
		InitializeWall();

		SetPlayerToStartPosition();

		BaseStageUIManager.Instance.ShowMessage(MessageUtils.StageDataLoadSuccessMessage);
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



	void SetPlayerToStartPosition() {
		if (startTransform)
			player.transform.position = startTransform.position;
	}

	public void TouchGoal() {
		UIManager.ShowMessage("クリアしました");
	}

	public void DiePlayer() {
		UIManager.ShowMessage("死亡しました");
	}
}

public class Game {
	public bool isStarted;
	public float limitTime;
	public uint limitCreate;
	public uint limitBreak;

	public bool IsTimeOver {
		get { return limitTime <= 0f; }
	}

	public Game(Stage stage) {
		isStarted = false;
		limitTime = stage.timeLimit;
		limitCreate = stage.createTime;
		limitBreak = stage.breakTime;
	}

	public void StartGame() {
		isStarted = true;
	}

	public void FinishGame() {
		isStarted = false;
	}
}

