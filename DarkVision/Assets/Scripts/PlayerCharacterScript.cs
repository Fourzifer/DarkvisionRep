using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour {

	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void MoveAbsolute(float x, float z) {
		rb.MovePosition(new Vector3(x, rb.position.y, z));
	}

	public void MoveRelative(float x, float z) {
		var rbPos = rb.position;
		MoveAbsolute(rbPos.x + x, rbPos.z + z);
	}

	public void Rotate(float degrees) {
		rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(degrees, Vector3.up));
	}

}
