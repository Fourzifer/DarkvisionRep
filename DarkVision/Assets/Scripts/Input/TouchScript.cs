using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Mouse = UnityEngine.InputSystem.Mouse;

public class TouchScript : MonoBehaviour
{

	public Vector2 Scale = Vector2.one;
	public Vector2 Offset = Vector2.one;

	[Space]
	public Rigidbody Player;
	public Vector2 PlayerSpeed = Vector2.one;

	void Update()
	{
		TouchInput();
	}

	void TouchInput()
	{
		if (Touch.activeFingers.Count > 0)
		{
			gameObject.transform.position = Touch.activeFingers[0].currentTouch.screenPosition;
			MovePlayer(Touch.activeFingers[0].currentTouch.delta);
			return;
		}

		if (Mouse.current.leftButton.isPressed)
		{
			gameObject.transform.position = Mouse.current.position.ReadValue() * Scale - Offset;

			if (!Mouse.current.leftButton.wasPressedThisFrame)
				MovePlayer(Mouse.current.delta.ReadValue());

		}

		// foreach (var touch in Touch.activeTouches)
		// Debug.Log($"{touch.touchId}: {touch.screenPosition},{touch.phase}");

	}

	void MovePlayer(Vector2 delta)
	{
		if (!Player)
		{
			return;
		}

		Player.MovePosition(Player.position + new Vector3(delta.x * PlayerSpeed.x, 0, delta.y * PlayerSpeed.y));

	}
}
