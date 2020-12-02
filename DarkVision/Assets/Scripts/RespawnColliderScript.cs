using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnColliderScript : MonoBehaviour {
	public GameObject Player;

	private Vector3 respawnPosition;

	void Start() {
		respawnPosition = Player.transform.position;
	}

	private void OnCollisionEnter(Collision other) {
		if (other.gameObject == Player) {
			Player.transform.position = respawnPosition;
			PopupHandlerScript.ShowPopup("respawn");
		}
	}

}
