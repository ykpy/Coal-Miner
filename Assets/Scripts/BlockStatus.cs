using UnityEngine;
using System.Collections;
using System.Linq;

public class BlockStatus : MonoBehaviour {

	public Block blockType;

	public StageIndex stageIndex;

	// Use this for initialization
	void Start () {
		foreach (var block in GameObject.FindGameObjectsWithTag(Tags.BLOCK).Where(block => block.transform.position == BlockUtils.RoundPosition(transform.position))
			.Where(block => block != gameObject).ToArray())
			Destroy(block);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
