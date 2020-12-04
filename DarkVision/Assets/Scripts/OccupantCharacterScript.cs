using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OccupantCharacterScript : MonoBehaviour, Utility.IObserver<Vector3> {

	public float DistractionTime = 10f;

	private Vector3 returnPos;
	private Vector3 distractionPos;

	public float Speed = 10f;
	public float HearingDistance = 100f;

	private float timer = -1;

	void Start() {
		// PlayerCharacterScript.KnockEvent.Add(this);
		this.Register(PlayerCharacterScript.KnockEvent);		

		returnPos = transform.position;
	}

	private void OnDestroy() {
		// PlayerCharacterScript.KnockEvent.Remove(this);
		this.Deregister(PlayerCharacterScript.KnockEvent);
	}

	void Update() {
		if (timer > 0) {
			transform.position = Vector3.MoveTowards(transform.position, distractionPos, Speed * Time.deltaTime);
			timer -= Time.deltaTime;
		} else {
			transform.position = Vector3.MoveTowards(transform.position, returnPos, Speed * Time.deltaTime);
		}
	}

	public void Notify(Vector3 knockPosition) {
		Vector3 projectedKnockPosition = new Vector3(knockPosition.x, returnPos.y, knockPosition.z);
		// TODO: check if close enough to hear
		if ((transform.position - projectedKnockPosition).sqrMagnitude > HearingDistance * HearingDistance) {
			return;
		}
		// TODO: raycast if there are things in the way, check distance between knock and raycast hit?
		
		distractionPos = projectedKnockPosition;
		timer = DistractionTime;
	}
}
