using UnityEngine;
using System.Collections;
using System.Linq;

public class BlockStatus : MonoBehaviour {

	public Block blockType;

	public StageIndex stageIndex;

	public bool singleton = false;

	// Use this for initialization
	void Start () {
		var allBlocks = GameObject.FindGameObjectsWithTag(Tags.BLOCK);
		foreach (var block in allBlocks.Where(block => block.transform.position == BlockUtils.RoundPosition(transform.position))
			.Where(block => block != gameObject).ToArray())
			Destroy(block);

		if (singleton) {
			foreach (var block in allBlocks.Where(block => block != gameObject && block.GetComponent<BlockStatus>().blockType == blockType)) {
				Destroy(block);
				StageManager.Instance.EraseBlock(block.GetComponent<BlockStatus>().stageIndex);
			}
		}
	}

}
