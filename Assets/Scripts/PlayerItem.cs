using UnityEngine;

public class PlayerItem : MonoBehaviour {

	public float rayDistance = 1f;

	GameObject targetBlock;
	Color defaultColor;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var ray = CameraSwitcher.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, rayDistance)) {

			if (targetBlock != hit.collider.gameObject) {
				if (targetBlock)
					targetBlock.GetComponent<Renderer>().material.color = defaultColor;
				targetBlock = hit.collider.gameObject;
				defaultColor = targetBlock.GetComponent<Renderer>().material.color;
				targetBlock.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 0.0f);
			}

			if (Input.GetMouseButtonDown(0)) {
				if (StageManager.Instance.UseBlockCreate()) {
					var instantPosition = BlockUtils.GetSurface(hit);
					Instantiate(StageManager.Instance.breakableBlock, BlockUtils.RoundPosition(instantPosition), Quaternion.identity);
					AudioManager.Instance.PlaySoundEffect(0);
				}
			} else if (Input.GetMouseButtonDown(1)) {
				if (StageManager.Instance.UseBlockBreak(targetBlock)) {
					Destroy(targetBlock);
					AudioManager.Instance.PlaySoundEffect(1);
				}
			}
		}

	}
}
