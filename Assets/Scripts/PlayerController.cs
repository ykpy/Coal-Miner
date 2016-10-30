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
	bool jumping = false;

	CapsuleCollider capsuleCollider;

	void Awake() {
		moveDirection = Vector3.zero;

		rb = GetComponent<Rigidbody>();

		mouseLook.Init(transform, cam.transform);

		capsuleCollider = GetComponent<CapsuleCollider>();
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

		moveDirection = cam.transform.forward * Input.GetAxisRaw("Vertical")
						+ cam.transform.right * Input.GetAxisRaw("Horizontal");
		moveDirection *= moveSpeed;

		moveDirection.y = rb.velocity.y;

		if (grounded) {
			if (jump) {
				moveDirection.y = jumpPower;
			}
		}
		jump = false;


		rb.velocity = moveDirection;
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

		if (!prevGrounded && grounded && jumping) {
			jumping = false;
		} 
	}

}
