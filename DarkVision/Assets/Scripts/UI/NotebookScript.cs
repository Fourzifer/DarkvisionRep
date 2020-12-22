using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotebookScript : MonoBehaviour {

	private static NotebookScript mainInstance = null;

	[Serializable]
	public class NotebookEntry {
		// IDEA: give entries keys or ids for updating existing entries (and moving them to the front) instead of just adding a new entry
		// IDEA: use dictionary
		public string Text = "";
		public AudioClip Clip;
		public int CurrentPriority = 0;
		public bool Enabled = true;

		public NotebookEntry(string text, AudioClip clip, bool enabled = true, int priority = 0) {
			Text = text;
			Clip = clip;
			CurrentPriority = priority;
			Enabled = enabled;
		}
	}

	private List<NotebookEntry> entries;
	public NotebookEntry StartEntry;
	private int currentIndex = 0;

	public TMP_Text NoteText;


	// float timer = -1;

	void Start() {
		mainInstance = this;
		gameObject.SetActive(false);
		entries = new List<NotebookEntry>();
		entries.Add(StartEntry);
	}

	private void OnDestroy() {
		if (mainInstance == this)
			mainInstance = null;

	}

	// void Update() {
	// 	if (timer > 0) {
	// 		timer -= Time.deltaTime;
	// 		if (timer < 0)
	// 			gameObject.SetActive(false);
	// 	}
	// }

	private void PlayEntry(int index) {
		var entry = mainInstance.entries[index];

		// PopupHandlerScript.ShowCustomPopup(entry.Text, entry.Clip.length);

		NoteText.text = entry.Text;

		PlayerCharacterScript.StopNarratorNow();
		PlayerCharacterScript.PlayClip(entry.Clip);
	}

	public static void Show() {
		if (!mainInstance)
			return;

		mainInstance.gameObject.SetActive(true);
		// PlayerCharacterScript.PlayClip(mainInstance.entries[0].Clip);
		PlayLatest();
	}

	public static void Hide() {
		if (!mainInstance)
			return;

		PlayerCharacterScript.StopNarratorNow();
		mainInstance.gameObject.SetActive(false);
	}

	public static void PlayLatest() {
		if (!mainInstance)
			return;

		mainInstance.PlayEntry(mainInstance.entries.Count - 1);
	}

	public static void PlayLatestAsPopup() {
		if (!mainInstance)
			return;

		var entry = mainInstance.entries[mainInstance.entries.Count - 1];

		if (entry.Clip == null)
		{
			Debug.LogWarningFormat("Sound clip for \"{0}\" has not been assigned", entry.Text);
			return;
		}

		PopupHandlerScript.ShowCustomPopup(entry.Text, entry.Clip.length);
		PlayerCharacterScript.StopNarratorNow();
		PlayerCharacterScript.PlayClip(entry.Clip);
	}

	public static void PlayCurrent() {
		if (!mainInstance)
			return;

		mainInstance.PlayEntry(mainInstance.currentIndex);
	}

	public static void PlayNext() {
		if (!mainInstance)
			return;

		mainInstance.currentIndex--;
		if (mainInstance.currentIndex < 0) {
			mainInstance.currentIndex = 0;
			// TODO: play indication that end has been reached
			// IDEA: loop instead
			return;
		}

		PlayCurrent();

	}

	public static void PlayPrev() {
		if (!mainInstance)
			return;

	}


	public static void AddEntry(string dialogueKey) {
		var dialogueEntry = DialogueRegistryScript.GetEntry(dialogueKey);
		if (dialogueEntry == null) {
			Debug.LogWarningFormat("Key \"{0}\" does not exist in registry", dialogueKey);
			return;
		}

		mainInstance.entries.Add(new NotebookEntry(dialogueEntry.Dialogue, dialogueEntry.Clip));
	}

	public static void AddEntryIfHigherPriority(string dialogueKey, int priority, int questLine) {
		var dialogueEntry = DialogueRegistryScript.GetEntry(dialogueKey);
		if (dialogueEntry == null) {
			Debug.LogWarningFormat("Key \"{0}\" does not exist in registry", dialogueKey);
			return;
		}

		while (mainInstance.entries.Count <= questLine) {
			mainInstance.entries.Add(new NotebookEntry(dialogueEntry.Dialogue, dialogueEntry.Clip, enabled: false));
		}
		if (mainInstance.entries[mainInstance.currentIndex].CurrentPriority > priority) {
			Debug.LogFormat("Discarded new notebook entry with lower priority: {0}", dialogueKey);

			// NOTE: this line decides if the notebook still changes current questline when revisiting old dialogue
			mainInstance.currentIndex = questLine;
			
			return;
		}

		mainInstance.currentIndex = questLine;
		mainInstance.entries[questLine] = new NotebookEntry(dialogueEntry.Dialogue, dialogueEntry.Clip, true, priority);

	}

}
