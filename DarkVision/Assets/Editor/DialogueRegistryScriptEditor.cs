using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueRegistryScript))]
public class DialogueRegistryScriptEditor : Editor {

	SerializedProperty phrases;

	DialogueRegistryScript dialogueRegistry;

	List<bool> entriesFoldStates;

	private void Start() {
		phrases = serializedObject.FindProperty("Phrases");
		dialogueRegistry = (DialogueRegistryScript)target;
	}

	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();
		serializedObject.Update();
		phrases = serializedObject.FindProperty("Phrases");

		dialogueRegistry = (DialogueRegistryScript)target;

		// if (dialogueRegistry.Entries == null) {
		// 	dialogueRegistry.Entries = new List<DialogueRegistryScript.DialogueEntry>();
		// }

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Phrases");
		if (GUILayout.Button("Expand All")) {
			for (int i = 0; i < entriesFoldStates.Count; i++) {
				entriesFoldStates[i] = true;
			}
		}
		if (GUILayout.Button("Collapse All")) { 
			for (int i = 0; i < entriesFoldStates.Count; i++) {
				entriesFoldStates[i] = false;
			}
		}
		EditorGUILayout.EndHorizontal();

		if (entriesFoldStates == null) {
			entriesFoldStates = new List<bool>();
		}

		while (entriesFoldStates.Count < dialogueRegistry.Entries.Count) {
			entriesFoldStates.Add(false);
		}

		while (entriesFoldStates.Count > dialogueRegistry.Entries.Count) {
			entriesFoldStates.RemoveAt(entriesFoldStates.Count - 1);
		}

		GUIStyle style = new GUIStyle(EditorStyles.foldout);
		style.fixedWidth = 15;

		EditorGUI.BeginChangeCheck();

		EditorGUI.indentLevel += 1;
		for (int id = 0; id < dialogueRegistry.Entries.Count; id++) {
			var item = dialogueRegistry.Entries[id];

			EditorGUILayout.BeginHorizontal();
			entriesFoldStates[id] = EditorGUILayout.Foldout(entriesFoldStates[id], "Key:", style);
			item.Key = EditorGUILayout.TextField(item.Key);
			if (GUILayout.Button("Delete")) {
				dialogueRegistry.Entries.RemoveAt(id);
			}
			EditorGUILayout.EndHorizontal();
			if (entriesFoldStates[id]) {
				EditorGUI.indentLevel += 1;
				EditorGUILayout.LabelField("Dialogue:");
				// item.Dialogue = EditorGUILayout.TextField("Dialogue:",item.Dialogue);
				item.Dialogue = EditorGUILayout.TextArea(item.Dialogue);
				item.Clip = (AudioClip)EditorGUILayout.ObjectField("Clip:", item.Clip, typeof(AudioClip), false);
				EditorGUI.indentLevel -= 1;
			}
		}
		EditorGUI.indentLevel -= 1;

		if (GUILayout.Button("Add entry", GUILayout.Width(100), GUILayout.Height(30))) {
			dialogueRegistry.Entries.Add(new DialogueRegistryScript.DialogueEntry());
		}


		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(dialogueRegistry, "Change Dialogue Registry");
			EditorUtility.SetDirty(dialogueRegistry);
		}

		serializedObject.ApplyModifiedProperties();
		// if (serializedObject.ApplyModifiedProperties()) {
		// 	Undo.RecordObject(dialogueRegistry, "Change Dialogue Registry");
		// 	EditorUtility.SetDirty(dialogueRegistry);
		// }

	}
}
