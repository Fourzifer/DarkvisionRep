using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScript : MonoBehaviour {

	public bool AllowRotation = false;
	public bool InteractionEnabled = false;

	[Space]
	public Vector2 Scale = Vector2.one;
	public Vector2 Offset = Vector2.one;
	public GameObject SecondTrail;
	public GameObject Sphere;
	public GameObject SecondSphere;

	[Space]
	public PlayerCharacterScript Player;
	public Vector2 PlayerSpeed = Vector2.one;
	public float PlayerRotMod = 1;
	[Space]
	[Tooltip("How far the finger can move when tapping to still register as an interact tap")]
	public float TouchTapMaxDistance = 1f;
	[Tooltip("How much time the finger can take when holding to still register as an interact tap on release")]
	public float TouchTapMaxTime = .1f;

	[Space]
	public bool AllowMouse = true;
	public Vector2 MouseSpeedMod = Vector2.one;
	[Tooltip("How far the mouse can move between clicking and releasing and still register an interact action on release")]
	public float MouseInteractMaxDistance = 1f;
	[Tooltip("How much time the mouse can take between clicking and releasing and still register an interact action on release")]
	public float MouseInteractMaxTime = .1f;

	private Vector2 mainTouchPosBuffer;
	private Vector2 secondTouchPosBuffer;
	private Vector2 lastMainTouchLocation;
	private float angleBuffer = 0;

	private Vector2 mouseBuffer;
	private bool mouseClickBuffer = false;
	private Vector2 lastClickLocation;
	private float mouseClickTimer;

	private float clickTimer = -1;


	void FixedUpdate() {
		if (clickTimer > 0) {
			clickTimer -= Time.deltaTime;
		}

		if (TouchInput()) {

			return;
		}
		if (AllowMouse)
			MouseInput();
	}

	bool TouchInput() {

		if (!Player)
			return true;

		// TODO: same rotation behaviour for rotating with main finger as left finger
		// IDEA: choose finger to use for translate depending on if angle is clockwise or ccw, and on which finger is left or right

		int touchCount = Input.touchCount;

		if (touchCount < 1) {
			Sphere?.SetActive(false);
			SecondSphere?.SetActive(false);
			return false;
		}

		Sphere?.SetActive(true);

		Touch touch = Input.GetTouch(0);

		switch (touch.phase) {
			case TouchPhase.Began:
				clickTimer = TouchTapMaxTime;
				mainTouchPosBuffer = touch.position;
				lastMainTouchLocation = mainTouchPosBuffer;
				break;
			case TouchPhase.Moved: {
					Vector2 delta = touch.position - mainTouchPosBuffer;
					MovePlayer(delta);
					mainTouchPosBuffer = touch.position;
				}
				break;
			case TouchPhase.Ended:
				if (InteractionEnabled
					&& (touch.position - lastMainTouchLocation).sqrMagnitude < TouchTapMaxDistance * TouchTapMaxDistance
					&& clickTimer > 0) {
					Player.Interact();
				}
				break;
		}

		transform.localPosition = new Vector3(
			mainTouchPosBuffer.x * Scale.x + Offset.x,
			mainTouchPosBuffer.y * Scale.y + Offset.y,
			transform.localPosition.z
		);


		if (Input.touchCount > 1) {
			touch = Input.GetTouch(1);

			SecondSphere?.SetActive(true);

			switch (touch.phase) {
				case TouchPhase.Began: {
						secondTouchPosBuffer = touch.position;
						angleBuffer = Vector2.Angle(mainTouchPosBuffer, secondTouchPosBuffer);
					}
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary: {
						secondTouchPosBuffer = touch.position;
						float angle = Vector2.Angle(mainTouchPosBuffer, secondTouchPosBuffer);
						float angleDelta = angleBuffer - angle;
						angleBuffer = angle;

						if (AllowRotation)
							Player.Rotate(angleDelta * PlayerRotMod);
						// Debug.Log("rotated player " + angleDelta + " degrees");
					}
					break;
			}

			if (SecondTrail) {
				SecondTrail.transform.localPosition = new Vector3(
					secondTouchPosBuffer.x * Scale.x + Offset.x,
					secondTouchPosBuffer.y * Scale.y + Offset.y,
					SecondTrail.transform.localPosition.z
				);
			}

		} else {
			SecondSphere?.SetActive(false);
		}

		return true;

	}

	void MouseInput() {

		// if (!Input.GetMouseButton(0))
		// return;

		bool mouseDown = Input.GetMouseButton(0);
		bool mouseDownThisFrame = mouseDown && !mouseClickBuffer; //Input.GetMouseButtonDown(0);
		bool mouseUpThisFrame = !mouseDown && mouseClickBuffer;//Input.GetMouseButtonUp(0);
		Vector2 currentMousePosition = Input.mousePosition;

		if (mouseDownThisFrame) {
			// Debug.Log("click");
			clickTimer = MouseInteractMaxTime;
			lastClickLocation = currentMousePosition;
			mouseBuffer = currentMousePosition;
		}

		// TODO: click and hold
		// TODO: double tap
		// TODO: double tap hold

		if (mouseUpThisFrame) {

			float releaseDelta = (lastClickLocation - currentMousePosition).sqrMagnitude;
			float maxDistanceSquared = MouseInteractMaxDistance * MouseInteractMaxDistance;

			// Debug.Log("delta on release: " + releaseDelta + " (max: " + maxDistanceSquared + "), click timer: " + clickTimer);

			if (InteractionEnabled
				&& releaseDelta < maxDistanceSquared
				&& clickTimer > 0) {
				Player.Interact();
			}
		} else if (Input.GetMouseButton(0)) {
			Vector2 delta = currentMousePosition - mouseBuffer;
			MovePlayer(delta * MouseSpeedMod);
		}

		mouseBuffer = currentMousePosition;
		mouseClickBuffer = mouseDown;
	}

	void MovePlayer(Vector2 delta) {
		if (!Player) {
			return;
		}

		// TODO: walking sounds for touch and mouse input

		Player.MoveRelative(delta.x * PlayerSpeed.x, delta.y * PlayerSpeed.y, true);
	}

}
