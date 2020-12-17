#pragma warning disable 4014

using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Threading.Tasks;
using System.Text;
using Microsoft.CognitiveServices.Speech;
using System;
using Microsoft.CognitiveServices.Speech.Audio;

public class SRGSFileGenerator : EditorWindow {


	string outputFile = "file";

	List<string> entries = new List<string>();
	string emptyField = "";

	[MenuItem("DarkVision/SRGSFileGenerator")]
	private static void ShowWindow() {
		var window = GetWindow<SRGSFileGenerator>();
		window.titleContent = new GUIContent("SRGSFileGenerator");
		window.Show();
	}

	private void OnGUI() {

		EditorGUILayout.LabelField("Phrases:");
		for (int i = 0; i < entries.Count; i++) {
			entries[i] = EditorGUILayout.TextField(entries[i]);
		}

		// if (entries.Count > 1)
		entries.RemoveAll(entry => entry == "");

		emptyField = EditorGUILayout.TextField(emptyField);
		if (emptyField != "") {
			entries.Add(emptyField);
			emptyField = "";
		}


		// EditorGUILayout.Separator();
		EditorGUILayout.Space();

		outputFile = EditorGUILayout.TextField("Output file:", outputFile);
		if (GUILayout.Button("Generate File", GUILayout.Height(40))) {
			// TODO: get xml and write it to file
			string path = Application.dataPath + "/srgs/" + outputFile + ".xml";

		}

	}


	public string GenerateXML() {
		StringBuilder output = new StringBuilder("<speak xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xmlns:emo=\"http://www.w3.org/2009/10/emotionml\" version=\"1.0\" xml:lang=\"en-US\">");

		foreach (var item in entries) {

			// output.Append("<mstts:express-as style=\"" + item.VoiceStyle + "\">");


			// output.AppendFormat("<prosody rate=\"{0}%\" pitch=\"{1}%\">", item.Rate, item.Pitch);
			// output.Append(item.Text);
			// output.Append("</prosody></mstts:express-as></voice>");
		}

		output.Append("</speak>");

		var outString = output.ToString();

		Debug.Log("generated ssml xml for text-to-speech: " + outString);

		return outString;
	}
}
