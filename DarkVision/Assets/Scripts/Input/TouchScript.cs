using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScript : MonoBehaviour {

	public Vector2 Scale = Vector2.one;
	public Vector2 Offset = Vector2.one;
	public GameObject SecondTrail;

	[Space]
	public PlayerCharacterScript Player;
	public Vector2 PlayerSpeed = Vector2.one;
	public float PlayerRotMod = 1;

	private Vector2 mainTouchPosBuffer;
	private Vector2 secondTouchPosBuffer;
	private float angleBuffer = 0;

	void Update() {
		TouchInput();
	}

	void TouchInput() {

		if (!Player)
			return;

		// TODO: same rotation behaviour for rotating with main finger as left finger
		// IDEA: choose finger to use for translate depending on if angle is clockwise or ccw, and on which finger is left or right

		int touchCount = Input.touchCount;

		if (touchCount < 1)
			return;

		Touch touch = Input.GetTouch(0);

		if (touch.phase == TouchPhase.Began) {
			mainTouchPosBuffer = touch.position;
		}

		// Move the cube if the screen has the finger moving.
		if (touch.phase == TouchPhase.Moved) {
			Vector2 delta = touch.position - mainTouchPosBuffer;
			MovePlayer(delta);
			mainTouchPosBuffer = touch.position;
		}

		transform.localPosition = new Vector3(
			mainTouchPosBuffer.x * Scale.x + Offset.x,
			mainTouchPosBuffer.y * Scale.y + Offset.y,
			transform.localPosition.z
		);

		if (Input.touchCount == 2) {
			touch = Input.GetTouch(1);

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

		}



	}

	void MovePlayer(Vector2 delta) {
		if (!Player) {
			return;
		}

		Player.MoveRelative(delta.x * PlayerSpeed.x, delta.y * PlayerSpeed.y);

	}

}
