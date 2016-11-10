using System.Linq;
using UnityEngine;

public class Stage {

	public static readonly string StageDataDirectoryPath = Application.dataPath + @"/../StageData/";


	string stageName;
	public string StageName {
		get {
			return stageName;
		}
		set {
			stageName = value;
		}
	}

	public uint breakTime;
	public uint createTime;

	public int timeLimit;

	Block[,,] blocks;
	uint x;
	uint y;
	uint z;

	public uint X {
		get { return x; }
	}

	public uint Y {
		get { return y; }
	}

	public uint Z {
		get { return z; }
	}

	public Stage(uint x, uint y, uint z) {

		this.x = x;
		this.y = y;
		this.z = z;

		blocks = GetZeroArray(x, y, z);
	}

	public Stage(uint x, uint y, uint z, Stage stage) {
		this.x = x;
		this.y = y;
		this.z = z;

		blocks = GetZeroArray(x, y, z);

		for (uint i = 0; i < x; i++) {
			for (uint j = 0; j < y; j++) {
				for (uint k = 0; k < z; k++) {
					if (i < stage.x && j < stage.y && k < stage.z) {
						blocks[i, j, k] = stage[i, j, k];
					}
				}
			}
		}
	}

	public Stage(string stageData) {
		var lines = stageData.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
			.Where(line => !line.StartsWith("#"))	// コメント行を取り除く
			.ToArray();

		// ステージ名
		stageName = lines[0];

		timeLimit = int.Parse(lines[1]);

		// ブロック生成・破壊可能回数
		var times = lines[2].Split(' ').Select(time => uint.Parse(time)).ToArray();
		createTime = times[0];
		breakTime = times[1];

		// ステージサイズの読み込み
		var stageSize = lines[3].Split(' ').Select(size => uint.Parse(size)).ToArray();
		x = stageSize[0];
		y = stageSize[1];
		z = stageSize[2];

		blocks = new Block[x, y, z];

		// ステージデータ
		for (uint j = 0; j < y; j++) {
			for (uint k = 0; k < z; k++) {
				var xLine = lines[j * z + k + 4]
							.Split(new[] {' '}, System.StringSplitOptions.RemoveEmptyEntries)
							.Select(l => uint.Parse(l)).ToArray();
				for (uint i = 0; i < x; i++) {
					blocks[i, j, k] = (Block) xLine[i];
				}
			}
		}
	}

	public Block this[uint x, uint y, uint z] {
		get {
			return blocks[x, y, z];
		}

		set {
			blocks[x, y, z] = value;
		}
	}

	Block[,,] GetZeroArray(uint x, uint y, uint z) {
		Block[,,] blocks = new Block[x, y, z];

		for (uint i = 0; i < x; i++) {
			for (uint j = 0; j < y; j++) {
				for (uint k = 0; k < z; k++) {
					blocks[i, j, k] = Block.Empty;
				}
			}
		}

		return blocks;
	}

	public int GetCoinCount() {
		int count = 0;
		for (uint i = 0; i < x; i++) {
			for (uint j = 0; j < y; j++) {
				for (uint k = 0; k < z; k++) {
					if (blocks[i, j, k] == Block.Coin)
						count++;
				}
			}
		}
		return count;
	}
}

public class StageDataTag {
	public const string StageName = "StageName";
	public const string TimeLimit = "TimeLimit";
	public const string BlockCreateBreakNum = "Create Break";
	public const string StageSize = "StageSize X Y Z";
	public const string Stage = "Stage";
}