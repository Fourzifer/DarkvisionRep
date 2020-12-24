using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotebookEventRegistryScript : MonoBehaviour {

	[Serializable]
	public class NotebookEventEntry {
		public string Key;
		public string DialogueKey;
		public int Priority;
		public int QuestLine;
	}

	public List<NotebookEventEntry> Entries = new List<NotebookEventEntry>();


	public void ApplyEntry(string key) {

		var entry = Entries.FirstOrDefault(firstEntry => firstEntry.Key == key);

		if (entry == null) {
			Debug.LogWarningFormat("Notebook entry \"{0}\" does not exist", key);
			return;
		}

		NotebookScript.AddEntryIfHigherPriority(entry.DialogueKey, entry.Priority, entry.QuestLine);
	}

	public void DisableQuestline(int index) {
		NotebookScript.DisableQuestlineStatic(index);
	}

}
