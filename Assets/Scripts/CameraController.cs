using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float moveSpeed;
	Vector3 moveDirection = Vector3.zero;

	public float rotateSpeed = 10f;

	Vector3 rotateAxis;

	Camera cam;

	float _wheelScroll = 0f;
	public float WheelScroll {
		get {
			return _wheelScroll;
		}
	}
	public int WheelScrollInvert = -1;

	void Start() {
		cam = GetComponent<Camera>();
	}

	void Update () {
		if (CameraSwitcher.Instance.MainCamera != cam)
			return;

		// カメラ移動
		if (!Input.GetKey(KeyCode.LeftShift)) {
			moveDirection.x = Input.GetAxisRaw("Horizontal");
			moveDirection.y = Input.GetAxisRaw("Vertical");
			moveDirection.z = WheelScroll * WheelScrollInvert;

			moveDirection = Vector3.zero;
			moveDirection += transform.right * Input.GetAxisRaw("Horizontal");
			moveDirection += transform.up * Input.GetAxisRaw("Vertical");
			moveDirection += transform.forward * WheelScroll * WheelScrollInvert;
		} else {
			transform.rotation *= Quaternion.Euler(Input.GetAxisRaw("Vertical") * -rotateSpeed * Time.deltaTime, Input.GetAxisRaw("Horizontal") * rotateSpeed * Time.deltaTime, 0);
		}
		transform.position += moveDirection * moveSpeed * Time.deltaTime;

	}

	void OnGUI() {
		if (Event.current.type == EventType.ScrollWheel) {
			_wheelScroll = Event.current.delta.y;
		} else {
			_wheelScroll = 0f;
		}
	}
}
