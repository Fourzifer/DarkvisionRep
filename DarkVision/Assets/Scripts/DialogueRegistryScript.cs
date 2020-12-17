using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueRegistryScript : MonoBehaviour {

	private static DialogueRegistryScript mainInstance = null;

	[Serializable]
	public class DialogueEntry {
		public string Key = "";
		public string Dialogue = "";
		public AudioClip Clip;
		// public float Time = 10;
	}

	public List<DialogueEntry> Entries = new List<DialogueEntry>();

	// void Start() {
	private void Awake() {
		mainInstance = this;
		// if (Entries == null) {
		// 	Entries = new List<DialogueEntry>();
		// }
	}

	private void OnDestroy() {
		// if (mainInstance == this) {
		// 	mainInstance = null;
		// }
	}

	public static DialogueEntry GetEntry(string key) {
		if (!mainInstance || mainInstance.Entries == null) {
			return null;
		}

		return mainInstance.Entries.FirstOrDefault(firstEntry => firstEntry.Key == key);
	}

}
