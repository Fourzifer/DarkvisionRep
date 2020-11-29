using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCListenerScript : MonoBehaviour, Utility.IObserver<(Vector3, string)> {

	public float HearingDistance = 15;
	// TODO: Voices and event hook entries for each phrase
	public string[] Phrases;

	void Start() {
		Utility.Register(this, MicListenerScript.SpeakEvent);
	}

	private void OnDestroy() {
		Utility.Deregister(this, MicListenerScript.SpeakEvent);
	}

	public void Notify((Vector3, string) notification) {
		Vector3 pos = notification.Item1;

		float distance = Vector3.Distance(transform.position, pos);
		Debug.Log("NPC distance: " + distance);


		if (distance > HearingDistance) {
			// Too far away to hear
			return;
		}

		string word = notification.Item2;

		if (Phrases.Contains(word)) {
			Debug.Log("I heard you say \"" + word + "\"! I am very interested in that");
		} else {
			Debug.Log("I don't know what \"" + word + "\" means");
		}
	}
}
