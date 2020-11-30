using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class NPCListenerScript : MonoBehaviour, Utility.IObserver<(Vector3, string)> {

	[Serializable]
	public class ListenPhraseEntry {
		public string Phrase;
		public AudioClip Clip;
		public UnityEvent Event;
	}

	public float HearingDistance = 15;

	public ListenPhraseEntry[] Phrases;

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
		ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Phrase == word);

		// if (Phrases.Select(entry => entry.Phrase).Contains(word)) {
		if (entry != null) {
			Debug.Log("I heard you say \"" + word + "\"! I am very interested in that");

			if (entry.Clip)
				PlayerCharacterScript.PlayClip(entry.Clip);

			entry.Event.Invoke();
		} else {
			Debug.Log("I don't know what \"" + word + "\" means");
		}
	}
}
