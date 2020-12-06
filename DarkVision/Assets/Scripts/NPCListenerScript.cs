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
		if (!narrator) {
			narrator = gameObject.AddComponent<AudioSource>();
			narrator.playOnAwake = false;
			narrator.spatialBlend = 1.0f;
		}
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

		if (entry != null) {
			Debug.Log("[In response to \"" + word + "\"]: " + entry.Response);

			PopupHandlerScript.ShowCustomPopup(entry.Response);
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
}

// NOTE: uncomment the multiline comment below to revert to default inspector
//*
[CustomEditor(typeof(NPCListenerScript))]
public class NPCListenerScriptEditor : Editor {

	SerializedProperty phrases;

	NPCListenerScript listenerScript;

	private bool showDebug = false;

	private void Start() {
		phrases = serializedObject.FindProperty("Phrases");
		listenerScript = (NPCListenerScript)target;
	}

	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();
		serializedObject.Update();
		phrases = serializedObject.FindProperty("Phrases");

		listenerScript = (NPCListenerScript)target;

		listenerScript.HearingDistance = EditorGUILayout.FloatField("Hearing distance", listenerScript.HearingDistance);
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Phrases");
		for (int id = 0; id < listenerScript.Phrases.Count; id++) {
			var item = listenerScript.Phrases[id];

			EditorGUILayout.BeginHorizontal();
			item.Hidden = !EditorGUILayout.Foldout(!item.Hidden, "id: " + id);
			item.Enabled = EditorGUILayout.Toggle(item.Enabled, GUILayout.Width(30));
			// EditorGUILayout.LabelField("id: " + id, GUILayout.Width(40));
			item.Phrase = EditorGUILayout.TextField(item.Phrase);
			if (GUILayout.Button("Remove")) {
				listenerScript.Phrases.Remove(item);
			}
			EditorGUILayout.EndHorizontal();

			if (!item.Hidden) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.PrefixLabel("Response:");
				item.Response = EditorGUILayout.TextArea(item.Response, GUILayout.Height(40));
				item.Clip = (AudioClip)EditorGUILayout.ObjectField("Clip", item.Clip, typeof(AudioClip), false);
				if (phrases != null) {
					EditorGUILayout.PropertyField(phrases.GetArrayElementAtIndex(id).FindPropertyRelative("Event"));
				} else {
					EditorGUILayout.LabelField("Warning: phrases ref is null");
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		if (GUILayout.Button("Add phrase", GUILayout.Width(100))) {
			listenerScript.Phrases.Add(new NPCListenerScript.ListenPhraseEntry());
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Default response:");
		listenerScript.DefaultResponse = EditorGUILayout.TextArea(listenerScript.DefaultResponse);
		listenerScript.DefaultResponseClip = (AudioClip)EditorGUILayout.ObjectField("Clip", listenerScript.DefaultResponseClip, typeof(AudioClip), false);

		EditorGUILayout.Space();
		showDebug = EditorGUILayout.Foldout(showDebug, "Debug options");
		if (showDebug) {
			listenerScript.PrintHearingDistance = EditorGUILayout.Toggle("Print distance", listenerScript.PrintHearingDistance);
		}

		serializedObject.ApplyModifiedProperties();


	}
}
// */
