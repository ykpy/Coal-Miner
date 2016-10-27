using System.Linq;

public class Stage {

	string stageName;
	public string StageName {
		get {
			return stageName;
		}
	}

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

		for (uint i = 0; i < stage.x; i++) {
			for (uint j = 0; j < stage.y; j++) {
				for (uint k = 0; k < stage.z; k++) {
					blocks[i, j, k] = stage[i, j, k];
				}
			}
		}
	}

	public Stage(string stageData) {
		var lines = stageData.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
		stageName = lines[0];

		var stageSize = lines[1].Split(' ').Select(size => uint.Parse(size)).ToArray();
		x = stageSize[0];
		y = stageSize[1];
		z = stageSize[2];

		blocks = new Block[x, y, z];

		for (uint j = 0; j < y; j++) {
			for (uint k = 0; k < z; k++) {
				var xLine = lines[j * z + k + 2]
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
}
