using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCListenerScript))]
public class NPCListenerScriptEditor : Editor {

	SerializedProperty phrases;

	NPCListenerScript listenerScript;

	private bool showDebug = false;

	private void Start() {
		phrases = serializedObject.FindProperty("Phrases");
		listenerScript = (NPCListenerScript)target;
	}

	private void OnSceneGUI() {
		listenerScript = (NPCListenerScript)target;
		Handles.color = Color.white;

		EditorGUI.BeginChangeCheck();
		listenerScript.HearingDistance = Handles.RadiusHandle(Quaternion.identity, listenerScript.transform.position, listenerScript.HearingDistance);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(listenerScript, "Change NPC hearing distance");
			EditorUtility.SetDirty(listenerScript);
		}
	}

	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();
		serializedObject.Update();
		phrases = serializedObject.FindProperty("Phrases");

		listenerScript = (NPCListenerScript)target;

		EditorGUI.BeginChangeCheck();
		listenerScript.HearingDistance = EditorGUILayout.FloatField("Hearing distance", listenerScript.HearingDistance);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(listenerScript, "Change NPC hearing distance");
			EditorUtility.SetDirty(listenerScript);
		}
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Phrases");

		EditorGUI.BeginChangeCheck();

		for (int id = 0; id < listenerScript.Phrases.Count; id++) {
			var item = listenerScript.Phrases[id];

			EditorGUILayout.BeginHorizontal();
			item.Hidden = !EditorGUILayout.Foldout(!item.Hidden, "id: " + id);
			item.Enabled = EditorGUILayout.Toggle(item.Enabled, GUILayout.Width(30));
			// EditorGUILayout.LabelField("id: " + id, GUILayout.Width(40));

			if (item.Phrase == null) {
				item.Phrase = new List<string>();
				Debug.LogWarning("Created new phrase list for entry");
			}

			if (!item.MultiplePhrases) {
				if (item.Phrase.Count < 1) {
					item.Phrase.Add("");
				}
				item.Phrase[0] = EditorGUILayout.TextField(item.Phrase[0]);

				if (GUILayout.Button("Remove")) {
					listenerScript.Phrases.Remove(item);
				}
				EditorGUILayout.EndHorizontal();
			} else {
				if (GUILayout.Button("Remove entry", GUILayout.Width(140))) {
					listenerScript.Phrases.Remove(item);
				}
				EditorGUILayout.EndHorizontal();

				EditorGUI.indentLevel += 1;
				// for (int i = item.Phrase.Count - 1; i >= 0 ; i--){
				for (int i = 0; i < item.Phrase.Count; i++) {
					EditorGUILayout.BeginHorizontal();
					item.Phrase[i] = EditorGUILayout.TextField(item.Phrase[i]);
					if (!item.Hidden && GUILayout.Button("Remove phrase", GUILayout.Width(120))) {
						item.Phrase.RemoveAt(i);
					}
					EditorGUILayout.EndHorizontal();
				}

				EditorGUI.indentLevel -= 1;
			}



			if (!item.Hidden) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.BeginHorizontal();
				item.MultiplePhrases = EditorGUILayout.Toggle("Multiple phrases", item.MultiplePhrases);
				if (item.MultiplePhrases && GUILayout.Button("Add phrase", GUILayout.Width(100))) {
					item.Phrase.Add("");
				}
				EditorGUILayout.EndHorizontal();

				item.ResponseKey = EditorGUILayout.TextField("Response Key",item.ResponseKey);
				if (phrases != null) {
					item.EventsHidden = !EditorGUILayout.Foldout(!item.EventsHidden, "Events");
					if (!item.EventsHidden) {
						EditorGUILayout.PropertyField(phrases.GetArrayElementAtIndex(id).FindPropertyRelative("Event"));
					}
				} else {
					EditorGUILayout.LabelField("Warning: phrases ref is null");
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		if (GUILayout.Button("Add entry", GUILayout.Width(100))) {
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

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(listenerScript, "Change NPC phrases");
			EditorUtility.SetDirty(listenerScript);
		}

		serializedObject.ApplyModifiedProperties();


	}
}
