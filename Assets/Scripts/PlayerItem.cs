using UnityEngine;

public class PlayerItem : MonoBehaviour {

	public float rayDistance = 1f;

	GameObject targetBlock;
	Color defaultColor;

	void Awake() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var ray = new Ray(CameraSwitcher.Instance.MainCamera.transform.position, CameraSwitcher.Instance.MainCamera.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, rayDistance)) {

			if (targetBlock != hit.collider.gameObject) {
				if (targetBlock)
					targetBlock.GetComponent<Renderer>().material.color = defaultColor;
				targetBlock = hit.collider.gameObject;
				defaultColor = targetBlock.GetComponent<Renderer>().material.color;
				targetBlock.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 0.0f);
			}

			if (Input.GetButtonDown("Create")) {
				if (StageManager.Instance.UseBlockCreate()) {
					var instantPosition = BlockUtils.GetSurface(hit);
					Instantiate(StageManager.Instance.BreakableBlock, BlockUtils.RoundPosition(instantPosition), Quaternion.identity);
					AudioManager.Instance.PlaySoundEffect(0);
				}
			} else if (Input.GetButtonDown("Break")) {
				if (StageManager.Instance.UseBlockBreak(targetBlock)) {
					Destroy(targetBlock);
					AudioManager.Instance.PlaySoundEffect(1);
				}
			}
		}

	}
}
