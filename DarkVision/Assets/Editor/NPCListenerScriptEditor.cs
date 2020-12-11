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
		listenerScript.HearingDistance = Handles.RadiusHandle(Quaternion.identity, listenerScript.transform.position, listenerScript.HearingDistance);
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
				item.PopupTime = EditorGUILayout.FloatField("Popup time", item.PopupTime);
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
