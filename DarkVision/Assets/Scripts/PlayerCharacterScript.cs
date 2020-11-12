using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour {

	public static List<Utility.IObserver<Vector3>> KnockEvent = new List<Utility.IObserver<Vector3>>();

	[SerializeField]
	[FMODUnity.EventRef] string walking = "event:/Walking";
	FMOD.Studio.EventInstance soundEvent;

	private enum InteractState {
		AboveWall,
		AboveDoor,
		None
	}

	private Rigidbody rb;

	public bool GamepadInput;
	public bool KbdInput;
	public float MoveSpeed = 10f;

	private bool interactableInRange = false;
	private bool interactPressedLastFrame = false;

	public Transform RaycastPosition;
	public LayerMask RaycastLayerMask;
	public float RaycastDistance = 10f;
	public int RaycastBufferSize = 4;

	private InteractState lastState = InteractState.None;

	void Start() {

		rb = GetComponent<Rigidbody>();
		soundEvent = FMODUnity.RuntimeManager.CreateInstance(walking);
		soundEvent.start();

		Debug.Log("Listing all joysticks: ");
		foreach (var item in Input.GetJoystickNames()) {
			Debug.Log(item);
		}
		Debug.Log("All joysticks listed");

	}

	private void FixedUpdate() {

		Ray ray = new Ray(RaycastPosition.position, RaycastPosition.forward);
		RaycastHit[] results = new RaycastHit[RaycastBufferSize];
		int resultCount = Physics.RaycastNonAlloc(ray, results, RaycastDistance, RaycastLayerMask);

		InteractState state = InteractState.None;
		for (int i = resultCount - 1; i >= 0; i--) {
			var result = results[i];
			switch (state) {
				case InteractState.AboveDoor:
					break;
				default:
					state = TagToState(result.transform.tag);
					break;
			}
		}

		if (state != lastState) {

			// TODO: change ground event

			Debug.Log("stepped from ground type |" + lastState.ToString() + "| onto ground type |" + state.ToString() + "|");

			lastState = state;
		}

		float x = 0;
		float z = 0;
		bool pressedInteract = false;

		if (KbdInput) {
			if (Input.GetKey(KeyCode.W)) {
				z = MoveSpeed * Time.deltaTime;
			} else if (Input.GetKey(KeyCode.S)) {
				z = -MoveSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D)) {
				x = MoveSpeed * Time.deltaTime;
			} else if (Input.GetKey(KeyCode.A)) {
				x = -MoveSpeed * Time.deltaTime;
			}

			if (Input.GetKey(KeyCode.E)) {
				pressedInteract = true;
			}
		}

		if (GamepadInput) {
			float xAxis = Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime;
			// float xAxis = Input.GetAxis("gamepadX") * MoveSpeed * Time.deltaTime;
			float yAxis = Input.GetAxis("Vertical") * MoveSpeed * Time.deltaTime;
			// float yAxis = Input.GetAxis("gamepadY") * MoveSpeed * Time.deltaTime;

			if (Mathf.Abs(xAxis) > Mathf.Abs(x)) {
				x = xAxis;
			}
			if (Mathf.Abs(yAxis) > Mathf.Abs(z)) {
				z = yAxis;
			}

			if (Input.GetButton("gamepadInteract")) {
				pressedInteract = true;
			}
		}

		{
			bool interactWasPressed = pressedInteract;
			if (pressedInteract) {
				if (interactPressedLastFrame) {
					pressedInteract = false;
				}
			}

			interactPressedLastFrame = interactWasPressed;
		}


		// if (KbdInput || GamepadInput) {
		// }

		if (pressedInteract) {
			Interact();
		}

		if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) {
			MoveRelative(x, z);

			soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(state));

		} else {

			soundEvent.setParameterByName("WalkingParameter", 0);//Idle sound

		}

	}

	public void Interact() {
		if (interactableInRange) {
			PopupHandlerScript.ShowPopup("look");
		} else {
			switch (lastState) {
				case InteractState.AboveWall:
					PopupHandlerScript.ShowPopup("wall");
					break;
				case InteractState.AboveDoor:
					PopupHandlerScript.ShowPopup("door");
					Utility.NotifyObservers(KnockEvent, transform.position);
					break;
				case InteractState.None:
					PopupHandlerScript.ShowPopup("room");
					break;
				default:
					break;
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

	private InteractState TagToState(string tag) {
		switch (tag) {
			case "Wall":
				return InteractState.AboveWall;
			case "Door":
				return InteractState.AboveDoor;
			default:
				return InteractState.None;
		}
	}

	private int StateToFmodValue(InteractState state) {
		switch (state) {
			case InteractState.AboveWall:
				return 1;
			case InteractState.AboveDoor:
				return 2;
			case InteractState.None:
				return 3;
		}
		return 0;
	}

}
