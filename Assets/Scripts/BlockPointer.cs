using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPointer : MonoBehaviour {

	RaycastHit hit;

	public GameObject cube;
	Renderer cubeRenderer;
	Color defaultColor;

	public float rayDistance = 10f;

	public Text pointerPositionText;

	StageIndex pointerIndex;

	GameObject targetBlock;

	Block blockType;

	public void SetGroundBlock() {
		this.blockType = Block.Ground;
		ChangeMaterialTexture();
	}

	public void SetBreakableBlock() {
		this.blockType = Block.Breakable;
		ChangeMaterialTexture();
	}

	public void SetUnbreakableBlock() {
		this.blockType = Block.Unbreakable;
		ChangeMaterialTexture();
	}

	public void SetStartBlock() {
		this.blockType = Block.Start;
		ChangeMaterialTexture();
	}

	public void SetGoalBlock() {
		this.blockType = Block.Goal;
		ChangeMaterialTexture();
	}

	public void SetCoin() {
		SetBlock(Block.Coin);
	}

	public void SetBlock(Block blockType) {
		this.blockType = blockType;
		ChangeMaterialTexture();
	}

	void ChangeMaterialTexture() {
		cubeRenderer.material.SetTexture("_MainTex", StageManager.Instance.GetTexture(blockType));
	}

	void Awake() {
		cubeRenderer = GetComponent<Renderer>();
	}

	// Use this for initialization
	void Start () {
		pointerIndex = ConvertPositionToIndex(transform.position);
		pointerPositionText.text = pointerIndex.ToString();

		SetGroundBlock();
	}
	
	// Update is called once per frame
	void Update () {

		var ray = CameraSwitcher.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, rayDistance)) {
			hit.point = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
			cube.transform.position = hit.point.Round();
		}

		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

		pointerIndex = ConvertPositionToIndex(transform.position);

		if (!IsUGUIHit()) {
			if (Input.GetMouseButtonDown(0)) {
				if (hit.collider != null) {
					StageManager.Instance.AddBlock(ConvertPositionToIndex(hit.point), blockType);
				}
			} else if (Input.GetMouseButton(1)) {
				// 右クリックされたままの状態のとき、ポインター先のブロックを取得する
				// ポインターを非表示にし、ポインター先のブロックの色を変化させる
				cubeRenderer.enabled = false;

				if (hit.collider == null || (hit.collider != null && hit.collider.gameObject != targetBlock)) {
					if (targetBlock) {
						targetBlock.GetComponent<Renderer>().material.color = defaultColor;
						targetBlock = null;
					}
				}
				if (hit.collider != null && hit.collider.tag == Tags.BLOCK && hit.collider.gameObject != targetBlock) {
					targetBlock = hit.collider.gameObject;
					defaultColor = targetBlock.GetComponent<Renderer>().material.color;
					targetBlock.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
				}
			} else if (Input.GetMouseButtonUp(1)) {
				if (targetBlock) {
					StageManager.Instance.EraseBlock(ConvertPositionToIndex(targetBlock.transform.position));
				}
				targetBlock = null;
				cubeRenderer.enabled = true;
			}
		}


		pointerPositionText.text = pointerIndex.ToString();
	}

	StageIndex ConvertPositionToIndex(Vector3 position) {
		return new StageIndex() {
			x = (uint) Mathf.RoundToInt(position.x),
			y = (uint) Mathf.RoundToInt(position.y),
			z = (uint) Mathf.RoundToInt(position.z)
		};
	}

	public bool IsUGUIHit() {
		var pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		var result = new System.Collections.Generic.List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, result);
		return result.Count > 0;
	}
}

[System.Serializable]
public struct StageIndex {
	public uint x;
	public uint y;
	public uint z;

	public override string ToString() {
		return string.Format("X: {0}  Y: {1}  Z: {2}", x, y, z);
	}
}

public static class VectorExtensions {
	public static Vector3 Round(this Vector3 self) {
		self.x = Mathf.Round(self.x);
		self.y = Mathf.Round(self.y);
		self.z = Mathf.Round(self.z);
		return self;
	}
}

