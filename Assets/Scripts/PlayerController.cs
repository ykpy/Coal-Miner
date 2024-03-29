﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	Vector3 moveDirection;
	public float jumpPower;
	public float moveSpeed;

	Rigidbody rb;

	public MouseLook mouseLook;

	public Camera cam;

	bool jump = false;
	bool grounded = false;
	bool prevGrounded = false;

	float fallDistance;
	Vector3 prevPosition;
	public float deadLineHeight = 1.5f;

	CapsuleCollider capsuleCollider;

	void Awake() {
		moveDirection = Vector3.zero;

		rb = GetComponent<Rigidbody>();

		mouseLook.Init(transform, cam.transform);

		capsuleCollider = GetComponent<CapsuleCollider>();
		prevPosition = transform.position;
	}

	void Update() {
		if (CameraSwitcher.Instance.MainCamera != cam)
			return;

		RotateView();

		if (!jump && Input.GetButtonDown("Jump")) {
			jump = true;
		}
	}

	void FixedUpdate() {
		CheckGround();

		if (!CameraSwitcher.Instance.IsMain(cam))
			return;

		moveDirection = transform.forward * Input.GetAxisRaw("Vertical")
						+ transform.right * Input.GetAxisRaw("Horizontal");
		moveDirection *= moveSpeed;

		moveDirection.y = rb.velocity.y;

		if (grounded) {
			if (jump) {
				moveDirection.y = jumpPower;
				fallDistance = deadLineHeight;
			}
		} else {
			var delta = prevPosition.y - transform.position.y;
			if (delta > 0f) {
				fallDistance -= Mathf.Abs(delta);
			}
		}
		jump = false;


		rb.velocity = moveDirection;

		prevPosition = transform.position;
	}

	void RotateView() {
		if (Mathf.Abs(Time.timeScale) < float.Epsilon)
			return;

		float oldYRotation = transform.eulerAngles.y;

		mouseLook.LookRotation(transform, cam.transform);

		Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
		rb.velocity = velRotation * rb.velocity;
	}

	void CheckGround() {
		prevGrounded = grounded;
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position, -transform.up, out hitInfo, capsuleCollider.height / 2)) {
			grounded = true;
		} else {
			grounded = false;
		}

		if (!prevGrounded && grounded) {
			if (fallDistance < 0f) {
				StageManager.Instance.DiePlayer();
			}
			fallDistance = deadLineHeight;
		} 
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == Tags.BLOCK) {
			var blockStatus = col.GetComponent<BlockStatus>();
			if (blockStatus.blockType == Block.Goal) {
				StageManager.Instance.TouchGoal();
			} else if (blockStatus.blockType == Block.Coin) {
				StageManager.Instance.TouchCoin();
				Destroy(col.gameObject);
			}
		} else if (col.tag == Tags.WALL) {
			StageManager.Instance.DiePlayer();
		}
	}
}
