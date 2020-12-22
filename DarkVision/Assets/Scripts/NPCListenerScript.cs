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
		// public float PopupTime = 10;
		public bool Enabled = true;
		public bool Hidden = false;
		public string ResponseKey = "";
		public bool EventsHidden = true;
		public UnityEvent Event;

		public bool ContainsPhrase(string phrase) {
			if (!Enabled || Phrase == null || Phrase.Count < 1)
				return false;

			char[] charsToTrim = { ' ', '.' };
			string trimmedPhrase = phrase.Trim(charsToTrim);

			if (MultiplePhrases)
				return Phrase.Contains(trimmedPhrase);

			return Phrase[0] == trimmedPhrase;
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

			var dialogueEntry = DialogueRegistryScript.GetEntry(entry.ResponseKey);
			if (dialogueEntry != null) {

				// Debug.Log("[In response to \"" + word + "\"]: " + dialogueEntry.Dialogue);
				float popupTime = 10;

				if (dialogueEntry.Clip) {
					popupTime = dialogueEntry.Clip.length;
					PlayerCharacterScript.StopNarratorNow();
					narrator?.Stop();
					narrator?.PlayOneShot(dialogueEntry.Clip);
				}

				PopupHandlerScript.ShowCustomPopup(
					dialogueEntry.Dialogue.Replace("\\n", "\n"),
					popupTime
				//entry.PopupTime
				);

				entry.Event.Invoke();
			} else {
				Debug.LogFormat("Key \"{0}\" not found in registry ", entry.ResponseKey);
			}
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

	public void QueueFModAudioClip(AudioClip clip) {
		EndOfClipEvents.Add(delegate { 
			narrator.PlayOneShot(clip);
		 });
	}

	public void QueueColliderDisable(Collider collider) {
		EndOfClipEvents.Add(delegate { collider.enabled = false; });
	}
	public void QueueColliderEnable(Collider collider) {
		EndOfClipEvents.Add(delegate { collider.enabled = true; });
	}


	public void AddNotebookEntry(string dialogueKey) {
		NotebookScript.AddEntry(dialogueKey);
	}
}
