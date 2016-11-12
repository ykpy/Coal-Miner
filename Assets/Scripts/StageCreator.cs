using UnityEngine;
using System.Collections.Generic;

public class StageCreator : MonoBehaviour {

	#region block
	public GameObject groundBlock;
	public GameObject unbreakableBlock;
	public GameObject breakableBlock;
	public GameObject startBlock;
	public GameObject goalBlock;
	public GameObject coin;
	#endregion

	public Transform stageTransform;

	public void InstantiateStage(Stage stage) {
		DestroyAllBlocks();
		for (uint j = 0; j < stage.Y; j++) {
			for (uint k = 0; k < stage.Y; k++) {
				for (uint i = 0; i < stage.X; i++) {
					InstantiateBlock(i, j, k, stage[i, j, k]);
				}
			}
		}
	}

	public GameObject InstantiateBlock(uint x, uint y, uint z, Block blockType) {
		if (blockType == Block.Empty)
			return null;
		var obj = Instantiate(GetBlockObjectByBlockType(blockType), new Vector3(x, y, z), Quaternion.identity) as GameObject;
		obj.GetComponent<BlockStatus>().stageIndex = new StageIndex() { x = x, y = y, z = z };
		obj.transform.SetParent(stageTransform);
		return obj;
	}

	/// <summary>
	/// 全てのブロックを破棄します
	/// </summary>
	public void DestroyAllBlocks() {
		foreach (var block in GameObject.FindGameObjectsWithTag(Tags.BLOCK)) {
			DestroyImmediate(block);
		}
	}

	/// <summary>
	/// ブロックタイプからブロックオブジェクトを取得します
	/// </summary>
	/// <param name="blockType">ブロックタイプ</param>
	/// <returns>タイプに対応するブロックゲームオブジェクト</returns>
	public GameObject GetBlockObjectByBlockType(Block blockType) {
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
			case Block.Coin:
				return coin;
			case Block.Empty:
				return null;
			default:
				throw new System.Exception();
		}
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
}
