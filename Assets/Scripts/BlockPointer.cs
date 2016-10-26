using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockPointer : MonoBehaviour {

	RaycastHit hit;

	public GameObject cube;

	public float rayDistance = 10f;

	public Text pointerPositionText;

	StageIndex pointerIndex;

	// Use this for initialization
	void Start () {
		pointerIndex = ConvertPositionToIndex(transform.position);
		pointerPositionText.text = pointerIndex.ToString();
	}
	
	// Update is called once per frame
	void Update () {

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, rayDistance)) {
			hit.point = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
			cube.transform.position = hit.point.Round();
		}

		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

		pointerIndex = ConvertPositionToIndex(transform.position);

		if (Input.GetMouseButtonDown(0)) {
			if (hit.collider != null) {
				StageManager.Instance.AddBlock(ConvertPositionToIndex(hit.point), Block.Breakable);
			}
		} else if (Input.GetMouseButtonDown(1)) {
			if (hit.collider.tag == Tags.BLOCK) {
				Destroy(hit.collider.gameObject);
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

