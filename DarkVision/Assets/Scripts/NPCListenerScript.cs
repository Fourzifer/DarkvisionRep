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
		public string Phrase;
		public bool Enabled = true;
		public bool Hidden = false;
		public string Response;
		public AudioClip Clip;
		public UnityEvent Event;
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
}

[CustomEditor(typeof(NPCListenerScript))]
public class NPCListenerScriptEditor : Editor {

	SerializedProperty phrases;

	NPCListenerScript listenerScript;

	private void Start() {
		phrases = serializedObject.FindProperty("Phrases");
		listenerScript = (NPCListenerScript)target;
	}

	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();
		serializedObject.Update();

		listenerScript = (NPCListenerScript)target;

		EditorGUILayout.LabelField("Phrases");
		for (int id = 0; id < listenerScript.Phrases.Count; id++) {
			EditorGUI.indentLevel += 1;
			var item = listenerScript.Phrases[id];

			EditorGUILayout.BeginHorizontal();
			item.Enabled = EditorGUILayout.Toggle(item.Enabled, GUILayout.Width(40));
			EditorGUILayout.LabelField("id: " + id, GUILayout.Width(40));
			item.Phrase = EditorGUILayout.TextField(item.Phrase);
			if (GUILayout.Button("Remove")) {
				listenerScript.Phrases.Remove(item);
			}
			EditorGUILayout.EndHorizontal();
			item.Response = EditorGUILayout.TextArea(item.Response);
			item.Clip = (AudioClip)EditorGUILayout.ObjectField("Clip", item.Clip, typeof(AudioClip), false);
			if (phrases != null) {
				EditorGUILayout.PropertyField(phrases.GetArrayElementAtIndex(id).FindPropertyRelative("Event"));
			} else {
				EditorGUILayout.LabelField("phrases ref is null");
			}
			EditorGUI.indentLevel -= 1;
		}
		if (GUILayout.Button("Add phrase")) {
			listenerScript.Phrases.Add(new NPCListenerScript.ListenPhraseEntry());
		}

		serializedObject.ApplyModifiedProperties();


	}
}
