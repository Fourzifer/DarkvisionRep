using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Mouse = UnityEngine.InputSystem.Mouse;
using static UnityEngine.InputSystem.InputAction;

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

		var touches = Touch.activeTouches;

		if (touches.Count > 0)
		{
			gameObject.transform.localPosition = touches[0].screenPosition;
			MovePlayer(touches[0].delta);
			return;
		}

		if (Mouse.current.leftButton.isPressed)
		{
			gameObject.transform.localPosition = Mouse.current.position.ReadValue() * Scale - Offset;

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


	public void OnFirstfinger(CallbackContext c)
	{
		Debug.Log("first finger: " + c.ToString());
	}

	public void OnSecondfinger(CallbackContext c)
	{
		Debug.Log("second finger: " + c.ToString());
	}

}
