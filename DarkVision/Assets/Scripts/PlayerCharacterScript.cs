using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour {

	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	public void MoveAbsolute(float x, float z) {
		rb.MovePosition(
			// transform.parent.position +
			// transform.localRotation * 
			new Vector3(x, transform.localPosition.y, z)
			);
	}

	public void MoveRelative(float x, float z) {
		var rbPos = transform.localPosition;
		var delta = transform.localRotation * new Vector3(x, 0, z);
		MoveAbsolute(rbPos.x + delta.x, rbPos.z + delta.z);
	}

	public void Rotate(float degrees) {
		rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(degrees, Vector3.up));
	}

}
