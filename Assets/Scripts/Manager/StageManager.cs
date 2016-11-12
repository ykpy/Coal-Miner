using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;


public class StageManager : SingletonMonoBehaviour<StageManager> {

	Stage stage;
	public Stage Stage {
		get { return stage; }
	}
	GameObject[,,] stageBlockObjects;

	Transform startTransform;
	public Transform stageTransform;

	[SerializeField]
	float sceneTransitionInterval = 1f;

	public GameObject player;

	public GameObject wall;
	readonly Vector3 defaultWallScale = new Vector3(1f, 0.01f, 1f);

	Game game;
	StageUIManager UIManager;

	StageCreator stageCreator;

	public bool edit = false;

	#region event
	public event System.Action<Block> OnBlockAdd = (blockType) => { };
	public event System.Action<Block> OnBlockErase = (blockType) => { };
	#endregion

	public GameObject BreakableBlock {
		get {
			return stageCreator.breakableBlock;
		}
	}

	protected override void Awake() {
		base.Awake();
		stageCreator = GetComponent<StageCreator>();

		UIManager = FindObjectOfType<StageUIManager>();
	}

	void Start() {
		InitializeStage(10, 10, 10);
		BaseStageUIManager.Instance.ShowStageInformation(stage);


		InitializeWall();

		SetPlayerToStartPosition();

		// ステージが選択されている場合、ゲームを開始する
		if (StageSelectManager.IsStageSelected()) {
			if (StageSelectManager.SelectedStageFileName != null) {
				LoadStage(StageSelectManager.SelectedStageFileName);
			} else {
				LoadStage(StageSelectManager.SelectedStage);
			}
			// ゲーム情報の生成
			game = new Game(stage);

			// UIのセット
			if (UIManager) {
				UIManager.ShowGameInformation(game);
				wall.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
			}

			StartGame();
		}
	}

	void Update() {
		if (game != null && game.isStarted) {
			if (!game.IsTimeOver) {
				game.limitTime -= Time.deltaTime;
				UIManager.ShowTimeLimit(Mathf.CeilToInt(game.limitTime));
				if (game.limitTime <= 10f && !game.isPinch) {
					AudioManager.Instance.PlayBGM(1);
					AudioManager.Instance.Loop = false;
					game.isPinch = true;
				}
			} else {
				if (game.isPinch) {
					TimeUp();
					game.isPinch = false;
				}
			}
		} else {
			if (Input.GetKeyDown(KeyCode.Return)) {
				StartGame();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadScene("stage");
		}
	}

	public void StartGame() {
		if (edit)
			return;

		game.StartGame();
		UIManager.ShowGameInformation(game);
		AudioManager.Instance.PlayBGM(0);
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
		stageCreator.DestroyAllBlocks();

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
		stageCreator.DestroyAllBlocks();
		this.stage = stage;
		stageBlockObjects = new GameObject[stage.X, stage.Y, stage.Z];

		DoToStage((s, i, j, k) => {
			AddBlock(new StageIndex { x = i, y = j, z = k }, s[i, j, k]);
		});

		return true;
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

		if (!IsInArea(x, y, z))
			return false;

		stage[x, y, z] = blockType;

		stageBlockObjects[x, y, z] = stageCreator.InstantiateBlock(x, y, z, blockType);

		if (blockType == Block.Start) {
			startTransform = stageBlockObjects[x, y, z].transform;
		}

		OnBlockAdd(blockType);

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
		OnBlockErase(stage[index.x, index.y, index.z]);
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

	public void LoadStage(Stage stage) {
		this.stage = stage;

		BaseStageUIManager.instance.ShowStageInformation(stage);
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

	public void RefreshScene() {
		DoToStage((stage, x, y, z) => {
			stage[x, y, z] = Block.Empty;
		});
		foreach (var block in GameObject.FindGameObjectsWithTag(Tags.BLOCK)) {
			var position = BlockUtils.RoundPosition(block.transform.position);
			var index = new StageIndex() { x = (uint) position.x, y = (uint) position.y, z = (uint) position.z };
			if (index.x < 0 || index.y < 0 || index.z < 0
				|| index.x >= stage.X || index.y >= stage.Y || index.z >= stage.Z) {

				Destroy(block);
			} else {
				stage[index.x, index.y, index.z] = block.GetComponent<BlockStatus>().blockType;
			}
		}
	}

	public void SetPlayerToStartPosition() {
		if (startTransform)
			player.transform.position = startTransform.position;
	}

	public void TouchGoal() {
		if (UIManager) {
			UIManager.ShowMessage("クリアしました");
			if (game.coinCount == game.coinMax) {
				UIManager.ShowMessage("完全クリアしました");
			}
		}
		StartCoroutine(GoToSelectScene(sceneTransitionInterval));
	}

	public void DiePlayer() {
		if (UIManager)
			UIManager.ShowMessage("死亡しました");
		StartCoroutine(GoToSelectScene(sceneTransitionInterval));
	}

	public void TimeUp() {
		if (UIManager)
			UIManager.ShowMessage("時間切れです");
		StartCoroutine(GoToSelectScene(sceneTransitionInterval));
	}

	public IEnumerator GoToSelectScene(float interval) {
		yield return new WaitForSeconds(interval);
		SceneManager.LoadScene("select");
	}

	public void TouchCoin() {
		if (UIManager) {
			game.coinCount++;
			UIManager.ShowMessage("コインをゲットしました");
			UIManager.ShowCoinInformation(game.coinCount, game.coinMax);
		}
	}

	public bool UseBlockCreate() {
		if (game.limitCreate > 0) {
			game.limitCreate--;
			if (UIManager)
				UIManager.ShowGameInformation(game);
			return true;
		}
		return false;
	}

	public bool UseBlockBreak(GameObject block) {
		if (game.limitBreak > 0) {
			if (block.GetComponent<BlockStatus>().blockType == Block.Breakable) {
				game.limitBreak--;
				if (UIManager)
					UIManager.ShowGameInformation(game);
				return true;
			}
		}
		return false;
	}

	public bool IsInArea(uint x, uint y, uint z) {
		return x < stage.X && y < stage.Y && z < stage.Z;
	}

	public Texture GetTexture(Block blockType) {
		return stageCreator.GetTexture(blockType);
	}
}

