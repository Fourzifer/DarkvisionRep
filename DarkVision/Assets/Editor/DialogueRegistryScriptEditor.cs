using System.Collections.Generic;
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

		EditorGUILayout.LabelField("Phrases");

		if (entriesFoldStates == null) {
			entriesFoldStates = new List<bool>();
		}

		while (entriesFoldStates.Count < dialogueRegistry.Entries.Count) {
			entriesFoldStates.Add(false);
		}

		while (entriesFoldStates.Count > dialogueRegistry.Entries.Count) {
			entriesFoldStates.RemoveAt(entriesFoldStates.Count - 1);
		}


		EditorGUI.BeginChangeCheck();

		EditorGUI.indentLevel += 1;
		for (int id = 0; id < dialogueRegistry.Entries.Count; id++) {
			var item = dialogueRegistry.Entries[id];

			EditorGUILayout.BeginHorizontal();
			entriesFoldStates[id] = EditorGUILayout.Foldout(entriesFoldStates[id], "");
			item.Key = EditorGUILayout.TextField("Key", item.Key);
			EditorGUILayout.EndHorizontal();
			if (entriesFoldStates[id]) {
				EditorGUI.indentLevel += 1;
				item.Dialogue = EditorGUILayout.TextField("Dialogue", item.Dialogue);
				item.Clip = (AudioClip)EditorGUILayout.ObjectField("", item.Clip, typeof(AudioClip), false);
				EditorGUI.indentLevel -= 1;
			}
		}
		EditorGUI.indentLevel -= 1;

		if (GUILayout.Button("Add entry", GUILayout.Width(100))) {
			dialogueRegistry.Entries.Add(new DialogueRegistryScript.DialogueEntry());
		}


		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(dialogueRegistry, "Change Dialogue Registry");
			EditorUtility.SetDirty(dialogueRegistry);
		}

		serializedObject.ApplyModifiedProperties();


	}
}
