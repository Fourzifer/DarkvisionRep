using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.Events;

public class NPCListenerScript : MonoBehaviour, Utility.IObserver<(Vector3, string)> {

	// IDEA: option to define different response for keywords that are not enabled yet, hinting that they might know something yet refuse to say

	[Serializable]
	public class ListenPhraseEntry {
		public List<string> Phrase;
		public bool MultiplePhrases = false;
		public float PopupTime = 10;
		public bool Enabled = true;
		public bool Hidden = false;
		public string Response;
		public AudioClip Clip;
		public UnityEvent Event;

		public bool ContainsPhrase(string phrase) {
			if (!Enabled || Phrase == null || Phrase.Count < 1)
				return false;

			// TODO: wildcard check?

			if (MultiplePhrases)
				return Phrase.Contains(phrase);

			return Phrase[0] == phrase;
		}
	}

	public float HearingDistance = 15;

	// [SerializeField]
	public List<ListenPhraseEntry> Phrases = new List<ListenPhraseEntry>();


	[Tooltip("What the NPC says in response to a phrase it does not recognize")]
	public string DefaultResponse;
	public AudioClip DefaultResponseClip;

	[Space]
	[Tooltip("Always print how far away the player is whenever a word is spoken, regardless of if they are in range or not")]
	public bool PrintHearingDistance = false;

	private AudioSource narrator;
	private List<Action> EndOfClipEvents = new List<Action>();

	void Start() {
		Utility.Register(this, MicListenerScript.SpeakEvent);

		narrator = GetComponent<AudioSource>();
		if (!narrator) {
			narrator = gameObject.AddComponent<AudioSource>();
			narrator.playOnAwake = false;
			narrator.spatialBlend = 1.0f;
		}
	}

	private void OnDestroy() {
		Utility.Deregister(this, MicListenerScript.SpeakEvent);
	}

	private void Update() {
		if (EndOfClipEvents.Any() && narrator && !narrator.isPlaying) {
			foreach (var item in EndOfClipEvents) {
				item.Invoke();
				Debug.Log("An end-of-clip event was invoked");
			}
			EndOfClipEvents.Clear();
		}
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
		ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.ContainsPhrase(word));

		if (entry != null) {
			Debug.Log("[In response to \"" + word + "\"]: " + entry.Response);

			PopupHandlerScript.ShowCustomPopup(entry.Response.Replace("\\n", "\n"), entry.PopupTime);
			if (entry.Clip) {
				PlayerCharacterScript.StopNarratorNow();
				narrator?.Stop();
				narrator?.PlayOneShot(entry.Clip);
			}

			entry.Event.Invoke();
		} else {
			Debug.Log("[Default response, \"" + word + "\" is either not recognised or enabled]: " + DefaultResponse);
			if (DefaultResponseClip)
				narrator?.PlayOneShot(DefaultResponseClip);
		}
	}

	public void EnablePhrase(int id) {
		// ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Phrase == phrase);
		// if (entry != null) {
		// 	entry.Enabled = true;
		// }
		if (id >= 0 && id < Phrases.Count)
			Phrases[id].Enabled = true;
	}

	public void DisablePhrase(int id) {
		// ListenPhraseEntry entry = Phrases.FirstOrDefault(firstEntry => firstEntry.Phrase == phrase);
		// if (entry != null) {
		// 	entry.Enabled = false;
		// }
		if (id >= 0 && id < Phrases.Count)
			Phrases[id].Enabled = false;
	}

	public void StopTalkingRightNow() {
		narrator?.Stop();
	}

	public void QueueFModEventRestart(Occlusion clip) {
		EndOfClipEvents.Add(delegate { clip.StartPlayBack(); });
	}
}
