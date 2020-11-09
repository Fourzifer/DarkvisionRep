using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour {

	[SerializeField]
	[FMODUnity.EventRef] string walking = "event:/Walking";
	FMOD.Studio.EventInstance soundEvent;

	private enum InteractState {
		AboveWall,
		AboveDoor,
		None
	}

	private Rigidbody rb;

	public bool KbdInput;
	public float KbdSpeed = 10f;

	private bool interactableInRange = false;

	public Transform RaycastPosition;
	public LayerMask RaycastLayerMask;
	public float RaycastDistance = 10f;
	public int RaycastBufferSize = 4;

	void Start() {
		rb = GetComponent<Rigidbody>();
		soundEvent = FMODUnity.RuntimeManager.CreateInstance(walking);
		soundEvent.start();
	}

	private void FixedUpdate() {
		if (KbdInput) {
			float x = 0;
			float z = 0;

			if (Input.GetKey(KeyCode.W)) {
				z = KbdSpeed * Time.deltaTime;
			} else if (Input.GetKey(KeyCode.S)) {
				z = -KbdSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D)) {
				x = KbdSpeed * Time.deltaTime;
			} else if (Input.GetKey(KeyCode.A)) {
				x = -KbdSpeed * Time.deltaTime;
			}

			// if (x > 0 && z > 0) 
			MoveRelative(x, z);

			if (Input.GetKey(KeyCode.E)) {
				if (interactableInRange == true) {
					PopupHandlerScript.ShowPopup("look");
				} else {
					switch (state) {
						case InteractState.AboveWall:
							PopupHandlerScript.ShowPopup("wall");
							break;
						case InteractState.AboveDoor:
							PopupHandlerScript.ShowPopup("door");
							break;
						default:
							break;
					}
				}
			}

			if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) {

				soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(state));

			} else {

				soundEvent.setParameterByName("WalkingParameter", 0);//Idle sound

			}

		}



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

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Interact")) {
			PopupHandlerScript.ShowPopup("interact");
			interactableInRange = true;
		}
	}

	// TODO: dont disable other popups/interactables still in range
	// IDEA: use ontriggerstay above instead
	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Interact")) {
			PopupHandlerScript.HidePopup("interact");
			interactableInRange = false;
		}
	}

}
