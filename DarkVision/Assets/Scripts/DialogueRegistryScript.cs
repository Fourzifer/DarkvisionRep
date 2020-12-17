using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueRegistryScript : MonoBehaviour {

	private static DialogueRegistryScript mainInstance = null;

	public class DialogueEntry {
		public string Key = "";
		public string Dialogue = "";
		public AudioClip Clip;
	}

	public List<DialogueEntry> Entries = new List<DialogueEntry>();

	void Start() {
		mainInstance = this;
	}

	private void OnDestroy() {
		if (mainInstance == this) {
			mainInstance = null;
		}
	}

	public static DialogueEntry GetEntry(string key) {
		if (!mainInstance || mainInstance.Entries == null) {
			return null;
		}

		return mainInstance.Entries.FirstOrDefault(firstEntry => firstEntry.Key == key);
	}

}
