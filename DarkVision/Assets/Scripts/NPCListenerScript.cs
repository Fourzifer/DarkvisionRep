using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class NPCListenerScript : MonoBehaviour, Utility.IObserver<(Vector3, string)> {

	// IDEA: option to define different response for keywords that are not enabled yet, hinting that they might know something yet refuse to say

	[Serializable]
	public class ListenPhraseEntry {
		public string Phrase;
		public bool Enabled = true;
		public string Response;
		public AudioClip Clip;
		public UnityEvent Event;
	}

	public float HearingDistance = 15;

	public ListenPhraseEntry[] Phrases;

	[Tooltip("What the NPC says in response to a phrase it does not recognize")]
	public string DefaultResponse;
	public AudioClip DefaultResponseClip;

	[Space]
	[Tooltip("Always print how far away the player is whenever a word is spoken, regardless of if they are in range or not")]
	public bool PrintHearingDistance = false;

	private AudioSource narrator;

	void Start() {
		Utility.Register(this, MicListenerScript.SpeakEvent);

		narrator = GetComponent<AudioSource>();
	}

	private void OnDestroy() {
		Utility.Deregister(this, MicListenerScript.SpeakEvent);
	}

	public void Notify((Vector3, string) notification) {
		Vector3 pos = notification.Item1;

		float distance = Vector3.Distance(transform.position, pos);

		if (PrintHearingDistance)
			Debug.Log("NPC distance: " + distance);

		if (distance > HearingDistance) {
			// Too far away to hear
			return;
		}

		string word = notification.Item2;
		ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Enabled && firstEntry.Phrase == word);

		// if (Phrases.Select(entry => entry.Phrase).Contains(word)) {
		if (entry != null) {
			Debug.Log("[In response to \"" + word + "\"]: " + entry.Response);

			if (entry.Clip)
				narrator?.PlayOneShot(entry.Clip);

			entry.Event.Invoke();
		} else {
			Debug.Log("[Default response, \"" + word + "\" is either not recognised or enabled]: " + DefaultResponse);
			if (DefaultResponseClip)
				narrator?.PlayOneShot(DefaultResponseClip);
		}
	}

	public void EnablePhrase(string phrase) {
		ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Phrase == phrase);
		if (entry != null) {
			entry.Enabled = true;
		}
	}

	public void DisablePhrase(string phrase) {
		ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Phrase == phrase);
		if (entry != null) {
			entry.Enabled = false;
		}
	}
}
