using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

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

	private class AnalyticsContainer {
		private Dictionary<string, object> data = new Dictionary<string, object>();

		// ~AnalyticsContainer(){
		// 	Debug.Log("Analytics destructor start");
		// 	Send();
		// 	Debug.Log("Analytics destructor end");
		// }

		public void AddOrUpdate(string key, object value) {
			if (data.ContainsKey(key)) {
				data[key] = value;
			} else {
				data.Add(key, value);
			}
		}

		public object GetObject(string key) {
			if (data.TryGetValue(key, out object result)) {
				return result;

			} else {
				Debug.LogErrorFormat("Analytics entry \"{0}\" does not exist yet", key);
				return null;
			}
		}

		public T Get<T>(string key) {
			if (data.TryGetValue(key, out object result)) {
				if (result is T) {
					return (T)result;
				} else {
					Debug.LogErrorFormat("Value/parameter of analytics entry \"{0}\" is not of type {1}", key, nameof(T));
					return default(T);
				}
			} else {
				Debug.LogErrorFormat("Analytics entry \"{0}\" does not exist yet", key);
				return default(T);
			}
		}

		public void Send() {
			if (data.Count < 1) {
				return;
			}
			AnalyticsEvent.Custom(
				"vent_crawler",
				data
			// new Dictionary<string, object> 
			// { 
			//     // {"Time", Mathf.RoundToInt(Timer)} 
			// } 
			);
			Debug.LogFormat("Sent {0} data entries", data.Count);
		}
	}

	private static AnalyticsContainer Analytics;

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
	public bool UseVelocityMovement = true;
	public float VelocityLimit = 10f;
	public float FootstepThreshold = .1f;

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

	private bool touchMovedThisFrame = false;

	// [Header("Notebook")]
	// [SerializeField]
	// private string currentNotebookHint = "I should go ask around";
	// [SerializeField]
	// private AudioClip notebookHintClip;
	private bool notebookKeyPressedLastFrame = false;
	private float notebookKeyTimer = 0;

	public float NotebookKeyTapTime = .3f;

	private bool updateInit = true;

	private Vector3 posBuffer;

	void Start() {

		rb = GetComponent<Rigidbody>();
		soundEvent = FMODUnity.RuntimeManager.CreateInstance(walking);
		soundEvent.start();

		Narrator = GetComponent<AudioSource>();

		if (Analytics == null) {
			Analytics = new AnalyticsContainer();
			// Analytics.AddOrUpdate("testtest", 42);
			Debug.Log("Analytics container created");
		}

	}

	private void OnDestroy() {
		Narrator = null;
	}

	private void OnApplicationQuit() {
		Analytics.Send();
		// Debug.Log("Analytics Sent");
		Analytics = null;
	}

	private void Update() {

		if (updateInit) {
			updateInit = false;
			if (Narrator) {
				PopupHandlerScript.ShowCustomPopup(WelcomeText, 20);
				Narrator.PlayOneShot(WelcomeClip);
				// NotebookScript.PlayLatestAsPopup();
			} else {
				Debug.LogError("Narrator wont play welcome clip! Narrator exists: " + (Narrator != null) + ", Welcome clip exists: " + (WelcomeClip != null));
			}
		}

		if (KbdInput) {
			// Movement
			moveDir = Direction.None;

			// Notebook
			// TODO: hold to show notebook
			// TODO: press other keys while held to navigate notebook, disabling movement

			bool notebookKeyPressed = Input.GetKey(KeyCode.F);
			if (notebookKeyPressed && !notebookKeyPressedLastFrame) {
				// NotebookScript.PlayLatestAsPopup();
				moveDir = Direction.None;
			}

			if (notebookKeyPressed) {
				notebookKeyTimer += Time.deltaTime;
				if (notebookKeyTimer > NotebookKeyTapTime) {
					NotebookScript.ShowIfHidden();
					if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
						Debug.Log("Playing next notebook entry");
						NotebookScript.PlayNext();
					} else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
						Debug.Log("Playing previous notebook entry");
						NotebookScript.PlayPrev();
					}
				}
			} else {

				if (notebookKeyPressedLastFrame) {

					NotebookScript.Hide();
					if (notebookKeyTimer < NotebookKeyTapTime) {
						NotebookScript.PlayLatestAsPopup();
					}
					notebookKeyTimer = 0;
				}

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

			notebookKeyPressedLastFrame = notebookKeyPressed;

			if (Input.GetKeyDown(KeyCode.P)) {
				Screen.fullScreen = !Screen.fullScreen;
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





		}
	}

	private void FixedUpdate() {


		float x = 0;
		float z = 0;

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

			// Interact
			// if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return)) {
			// 	pressedInteract = true;
			// }
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

		}


		if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0) {
			MoveRelative(x, z);
		}

		float deltaSquared = (posBuffer - transform.localPosition).sqrMagnitude;

		if (deltaSquared > FootstepThreshold * FootstepThreshold) {
			soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(InteractState.None));
			// Debug.Log("delta: "+ Mathf.Sqrt(deltaSquared));
		} else {
			soundEvent.setParameterByName("WalkingParameter", 0);
		}
		posBuffer = transform.localPosition;

		touchMovedThisFrame = false;
		// touchedVentWall = false;
	}

	private void LateUpdate() {
		rb.MoveRotation(Quaternion.RotateTowards(
			rb.rotation,
			Quaternion.AngleAxis(targetDirection, Vector3.up),
			RotationAnimationSpeed * Time.deltaTime)
		);

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

		// if (touchMove) {
		// 	if (touchedVentWall) {
		// 		soundEvent.setParameterByName("WalkingParameter", 0);
		// 	} else {
		// 		soundEvent.setParameterByName("WalkingParameter", StateToFmodValue(lastState));

		// 	}
		// }

		if (UseVelocityMovement) {
			rb.velocity = Quaternion.AngleAxis(targetDirection, Vector3.up) * new Vector3(x * 30f, rb.velocity.y, z * 30f);
			if (rb.velocity.sqrMagnitude > VelocityLimit * VelocityLimit) {
				rb.velocity = rb.velocity.normalized * VelocityLimit;
			}
		} else {
			var rbPos = transform.localPosition;
			var delta = transform.localRotation * new Vector3(x, 0, z);
			MoveAbsolute(rbPos.x + delta.x, rbPos.z + delta.z);
		}
	}

	public void Rotate(float degrees) {
		rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(degrees, Vector3.up));
	}

	// private void OnTriggerStay(Collider other) {
	// 	if (other.CompareTag("ventWall")) {
	// 		// touchedVentWall = true;
	// 	}
	// }

	// private void OnCollisionStay(Collision other) {
	// 	if (other.gameObject.CompareTag("ventWall")) {
	// 		// touchedVentWall = true;
	// 	}
	// }



	// private InteractState TagToState(string tag) {
	// 	switch (tag) {
	// 		case "Wall":
	// 			return InteractState.AboveWall;
	// 		case "Door":
	// 			return InteractState.AboveDoor;
	// 		default:
	// 			return InteractState.None;
	// 	}
	// }

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
		// currentNotebookHint = newNotebookHint;
		Debug.LogWarning("SetNoteBookText is obsolete, NPCListenerScript.AddNotebookEntry instead");
	}
	public void SetNotebookClip(AudioClip newNotebookHintClip) {
		// notebookHintClip = newNotebookHintClip;
		Debug.LogWarning("SetNoteBookClip is obsolete, NPCListenerScript.AddNotebookEntry instead");
	}

	// public void PlayNotebook() {
	// PopupHandlerScript.ShowCustomPopup(currentNotebookHint);
	// if (Narrator && notebookHintClip) {
	// 	Narrator.Stop();
	// 	Narrator.PlayOneShot(notebookHintClip);
	// }
	// }

	public static void PlayClip(AudioClip clip) {
		Narrator?.Stop();
		Narrator?.PlayOneShot(clip);
	}

	public static void StopNarratorNow() {
		Narrator?.Stop();
	}


}
