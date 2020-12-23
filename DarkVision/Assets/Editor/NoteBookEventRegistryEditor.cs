using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NotebookEventRegistryScript))]
public class NotebookEventRegistryEditor : Editor {

	SerializedProperty phrases;

	NotebookEventRegistryScript notebookRegistry;

	List<bool> entriesFoldStates;

	bool firstSelectedLast = false;
	int firstSelected = -1;
	int secondSelected = -1;

	private void Start() {
		phrases = serializedObject.FindProperty("Phrases");
		notebookRegistry = (NotebookEventRegistryScript)target;
	}

	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();
		serializedObject.Update();
		phrases = serializedObject.FindProperty("Phrases");

		notebookRegistry = (NotebookEventRegistryScript)target;

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

		while (entriesFoldStates.Count < notebookRegistry.Entries.Count) {
			entriesFoldStates.Add(false);
		}

		while (entriesFoldStates.Count > notebookRegistry.Entries.Count) {
			entriesFoldStates.RemoveAt(entriesFoldStates.Count - 1);
		}

		GUIStyle style = new GUIStyle(EditorStyles.foldout);
		style.fixedWidth = 15;

		EditorGUI.BeginChangeCheck();

		EditorGUI.indentLevel += 1;
		for (int id = 0; id < notebookRegistry.Entries.Count; id++) {
			var item = notebookRegistry.Entries[id];

			EditorGUILayout.BeginHorizontal();
			entriesFoldStates[id] = EditorGUILayout.Foldout(entriesFoldStates[id], "Key:", style);
			bool selected = id == firstSelected || id == secondSelected;
			bool toggleState = EditorGUILayout.Toggle(selected, GUILayout.Width(35), GUILayout.ExpandWidth(false));
			bool selectionChanged = selected != toggleState;
			if (selectionChanged) {
				if (selected) {
					if (!firstSelectedLast) {
						secondSelected = -1;
					} else {
						firstSelected = -1;
					}
				} else {
					if (firstSelectedLast) {
						secondSelected = id;
					} else {
						firstSelected = id;
					}
					firstSelectedLast = !firstSelectedLast;
				}
			}

			item.Key = EditorGUILayout.TextField(item.Key);
			if (GUILayout.Button("Delete")) {
				notebookRegistry.Entries.RemoveAt(id);
				if (firstSelected >= notebookRegistry.Entries.Count) {
					firstSelected = -1;
				}
				if (secondSelected >= notebookRegistry.Entries.Count) {
					secondSelected = -1;
				}
			}
			EditorGUILayout.EndHorizontal();
			if (entriesFoldStates[id]) {
				EditorGUI.indentLevel += 1;
				// EditorGUILayout.LabelField("Dialogue:");
				// item.Key = EditorGUILayout.TextField("Key", item.Key);
				item.DialogueKey = EditorGUILayout.TextField("Dialogue Key", item.DialogueKey);
				EditorGUILayout.BeginHorizontal();
				item.Priority = EditorGUILayout.IntField("Priority", item.Priority);
				item.QuestLine = EditorGUILayout.IntField("Questline", item.QuestLine);
				EditorGUILayout.EndHorizontal();
				// item.Clip = (AudioClip)EditorGUILayout.ObjectField("Clip:", item.Clip, typeof(AudioClip), false);
				EditorGUI.indentLevel -= 1;
			}
		}
		EditorGUI.indentLevel -= 1;

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add entry", GUILayout.Width(100), GUILayout.Height(30))) {
			notebookRegistry.Entries.Add(new NotebookEventRegistryScript.NotebookEventEntry());
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Swap selected")) {
			if (firstSelected >= 0 && secondSelected >= 0) {
				{
					var temp = notebookRegistry.Entries[firstSelected];
					notebookRegistry.Entries[firstSelected] = notebookRegistry.Entries[secondSelected];
					notebookRegistry.Entries[secondSelected] = temp;
				}

				{
					var temp = entriesFoldStates[firstSelected];
					entriesFoldStates[firstSelected] = entriesFoldStates[secondSelected];
					entriesFoldStates[secondSelected] = temp;
				}
			}
		}
		GUILayout.FlexibleSpace();
		// EditorGUILayout.LabelField(firstSelectedLast.ToString());

		EditorGUILayout.EndHorizontal();

		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(notebookRegistry, "Change Dialogue Registry");
			EditorUtility.SetDirty(notebookRegistry);
		}

		serializedObject.ApplyModifiedProperties();
		// if (serializedObject.ApplyModifiedProperties()) {
		// 	Undo.RecordObject(dialogueRegistry, "Change Dialogue Registry");
		// 	EditorUtility.SetDirty(dialogueRegistry);
		// }

	}
}
