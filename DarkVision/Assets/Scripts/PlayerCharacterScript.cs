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

	public enum MoveMode {
		Normal,
		Pacman,
		OnlyForwards,
		CorridorRelativeForwardsOnly
	}

	private enum Direction {
		None,
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest
	}

	private Rigidbody rb;

	private static AudioSource Narrator;
	public string WelcomeText;
	public AudioClip WelcomeClip;

	[Header("Input")]
	// public bool PacmanMode = true;
	public MoveMode Mode;
	public bool GamepadInput;
	public bool KbdInput;
	public float MoveSpeed = 10f;

	public bool InteractionEnabled = false;
	private bool interactableInRange = false;
	private bool interactPressedLastFrame = false;

	private bool rotateLeftPressedLastFrame = false;
	private bool rotateRightPressedLastFrame = false;
	private float targetDirection = 0;
	public float RotationAnimationSpeed = 250;
	public float RotationIncrementAmount = 90;

	private KeyCode lastMovementKeyPressed = KeyCode.None;
	private KeyCode[] allowedMovementKeys = {
		KeyCode.W,
		KeyCode.A,
		KeyCode.S,
		KeyCode.D,
		KeyCode.UpArrow,
		KeyCode.LeftArrow,
		KeyCode.DownArrow,
		KeyCode.RightArrow,
	};
	private Direction moveDir = Direction.None;
	private Direction facing = Direction.North;


	[Header("Raycast footsteps")]
	[Tooltip("The object whose center position represents the origin position of the ray")]
	public Transform RaycastPosition;
	[Tooltip("Which layers the raycast will react to/count collisions with")]
	public LayerMask RaycastLayerMask;
	[Tooltip("How far the ray will reach")]
	public float RaycastDistance = 10f;
	[Tooltip("How many results the raycast will keep, from closest to furthest hit from ray origin, the closest hit is always kept if the size is at least 1")]
	public int RaycastBufferSize = 4;

	private InteractState lastState = InteractState.None;
	private bool touchMovedThisFrame = false;
	private bool touchedVentWall = false;

	[Header("Notebook")]
	[SerializeField]
	private string currentNotebookHint = "I should go ask around";
	[SerializeField]
	private AudioClip notebookHintClip;
	private bool notebookKeyPressedLastFrame = false;

	void Start() {

		rb = GetComponent<Rigidbody>();
		soundEvent = FMODUnity.RuntimeManager.CreateInstance(walking);
		soundEvent.start();

		// Debug.Log("Listing all joysticks: ");
		// foreach (var item in Input.GetJoystickNames()) {
		// 	Debug.Log(item);
		// }
		// Debug.Log("All joysticks listed");

		Narrator = GetComponent<AudioSource>();
		if (Narrator && WelcomeClip) {
			PopupHandlerScript.ShowCustomPopup(WelcomeText, 20);
			Narrator.PlayOneShot(WelcomeClip);
		}

	}

	private void OnDestroy() {
		Narrator = null;
	}

	private void Update() {
		if (KbdInput) {
			// Movement
			moveDir = Direction.None;

			switch (Mode) {
				case MoveMode.Pacman:

					foreach (KeyCode key in allowedMovementKeys) {
						if (Input.GetKeyDown(key))
							lastMovementKeyPressed = key;
					}

					if (!Input.GetKey(lastMovementKeyPressed))
						lastMovementKeyPressed = KeyCode.None;


					switch (lastMovementKeyPressed) {
						case KeyCode.W:
						case KeyCode.UpArrow:
							moveDir = Direction.North;
							break;
						case KeyCode.A:
						case KeyCode.LeftArrow:
							moveDir = Direction.West;
							break;
						case KeyCode.S:
						case KeyCode.DownArrow:
							moveDir = Direction.South;
							break;
						case KeyCode.D:
						case KeyCode.RightArrow:
							moveDir = Direction.East;
							break;
						default:
							break;
					}
					break;
				case MoveMode.Normal:
					if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
						moveDir = Direction.North;
					} else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
						moveDir = Direction.South;
					}
					if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
						if (moveDir == Direction.North) {
							moveDir = Direction.NorthEast;
						} else if (moveDir == Direction.South) {
							moveDir = Direction.SouthEast;
						} else {
							moveDir = Direction.East;
						}
					} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
						if (moveDir == Direction.North) {
							moveDir = Direction.NorthWest;
						} else if (moveDir == Direction.South) {
							moveDir = Direction.SouthWest;
						} else {
							moveDir = Direction.West;
						}
					}
					break;
				case MoveMode.OnlyForwards:
					if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
						moveDir = Direction.North;
					} else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
						moveDir = Direction.South;
					}
					break;
				case MoveMode.CorridorRelativeForwardsOnly:

					switch (facing) {
						case Direction.North:
						case Direction.South:
							if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
								moveDir = Direction.North;
							} else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
								moveDir = Direction.South;
							}
							break;
						case Direction.West:
						case Direction.East:
							if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
								moveDir = Direction.East;
							} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
								moveDir = Direction.West;
							}
							break;
					}

					break;
			}


		}
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
			// Movement

			switch (moveDir) {
				case Direction.North:
					z = MoveSpeed * Time.deltaTime;
					break;
				case Direction.NorthEast:
					z = MoveSpeed * Time.deltaTime;
					x = MoveSpeed * Time.deltaTime;
					break;
				case Direction.NorthWest:
					z = MoveSpeed * Time.deltaTime;
					x = -MoveSpeed * Time.deltaTime;
					break;
				case Direction.South:
					z = -MoveSpeed * Time.deltaTime;
					break;
				case Direction.SouthEast:
					z = -MoveSpeed * Time.deltaTime;
					x = MoveSpeed * Time.deltaTime;
					break;
				case Direction.SouthWest:
					z = -MoveSpeed * Time.deltaTime;
					x = -MoveSpeed * Time.deltaTime;
					break;
				case Direction.East:
					x = MoveSpeed * Time.deltaTime;
					break;
				case Direction.West:
					x = -MoveSpeed * Time.deltaTime;
					break;
				default:
					break;
			}

			// Rotation
			bool rotateRightPressed = Input.GetKey(KeyCode.E);
			if (rotateRightPressed && !rotateRightPressedLastFrame)
				// rb.rotation *= Quaternion.AngleAxis(30, Vector3.up);
				targetDirection += RotationIncrementAmount;
			rotateRightPressedLastFrame = rotateRightPressed;

			bool rotateLeftPressed = Input.GetKey(KeyCode.Q);
			if (rotateLeftPressed && !rotateLeftPressedLastFrame)
				// rb.rotation *= Quaternion.AngleAxis(-30, Vector3.up);
				targetDirection -= RotationIncrementAmount;
			rotateLeftPressedLastFrame = rotateLeftPressed;

			targetDirection %= 360;
			if (targetDirection < 45 || targetDirection > 315) {
				facing = Direction.North;
			} else if (targetDirection < 135) {
				facing = Direction.East;
			} else if (targetDirection < 225) {
				facing = Direction.South;
			} else {
				facing = Direction.West;
			}

			// Interact
			if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return)) {
				pressedInteract = true;
			}

			// Notebook
			bool notebookKeyPressed = Input.GetKey(KeyCode.F);
			if (notebookKeyPressed && !notebookKeyPressedLastFrame)
				PlayNotebook();
			notebookKeyPressedLastFrame = notebookKeyPressed;
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

		if (InteractionEnabled && pressedInteract) {
			Interact();
		}

		if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) {
			MoveRelative(x, z);

			if (!touchMovedThisFrame) {
				if (touchedVentWall) {
					soundEvent.setParameterByName("WalkingParameter", 0);
				} else {
					soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(state));
				}
				// soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(state));
			}

		} else {

			if (!touchMovedThisFrame)
				soundEvent.setParameterByName("WalkingParameter", 0);//Idle sound

		}

		touchMovedThisFrame = false;
		touchedVentWall = false;
	}

	private void LateUpdate() {
		rb.MoveRotation(Quaternion.RotateTowards(
			rb.rotation,
			Quaternion.AngleAxis(targetDirection, Vector3.up),
			RotationAnimationSpeed * Time.deltaTime)
		);
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


	public void MoveRelative(float x, float z, bool touchMove = false) {
		touchMovedThisFrame = touchMove;

		if (touchMove) {
			if (touchedVentWall) {
				soundEvent.setParameterByName("WalkingParameter", 0);
			} else {
				soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(lastState));

			}
		}

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

	private void OnTriggerStay(Collider other) {
		if (other.CompareTag("ventWall")) {
			touchedVentWall = true;
		}
	}

	private void OnCollisionStay(Collision other) {
		if (other.gameObject.CompareTag("ventWall")) {
			touchedVentWall = true;
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

	public void SetNotebookText(string newNotebookHint) {
		currentNotebookHint = newNotebookHint;
	}
	public void SetNotebookClip(AudioClip newNotebookHintClip) {
		notebookHintClip = newNotebookHintClip;
	}

	public void PlayNotebook() {
		PopupHandlerScript.ShowCustomPopup(currentNotebookHint);
		if (Narrator && notebookHintClip) {
			Narrator.Stop();
			Narrator.PlayOneShot(notebookHintClip);
		}
	}

	public static void PlayClip(AudioClip clip) {
		Narrator?.Stop();
		Narrator?.PlayOneShot(clip);
	}

	public static void StopNarratorNow() {
		Narrator?.Stop();
	}


}
