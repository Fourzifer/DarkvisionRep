#pragma warning disable 4014

using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Threading.Tasks;
using System.Text;
using Microsoft.CognitiveServices.Speech;
using System;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;

public class SRGSFileGenerator : EditorWindow {


	string outputFile = "file";

	List<string> entries = new List<string>();
	string emptyField = "";

	[MenuItem("DarkVision/SRGS File Generator")]
	private static void ShowWindow() {
		var window = GetWindow<SRGSFileGenerator>();
		window.titleContent = new GUIContent("SRGS File Generator");
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

		// IDEA: option to read xml file to populate fields

		outputFile = EditorGUILayout.TextField("Output file:", outputFile);
		if (GUILayout.Button("Generate File", GUILayout.Height(40))) {
			// TODO: get xml and write it to file
			string path = Application.dataPath + "/srgs/" + outputFile + ".xml";
			string xml = GenerateXML();

			Debug.Log("Started writing srgs xml");
			File.WriteAllText(path, xml);
			Debug.Log("Done writing srgs xml");
			AssetDatabase.Refresh();

		}

	}


	public string GenerateXML() {
		StringBuilder output = new StringBuilder("<grammar version=\"1.0\" xml:lang=\"en-US\" root=\"rootRule\" xmlns=\"http://www.w3.org/2001/06/grammar\">");

		output.AppendLine("<rule id=\"rootRule\">");
		output.AppendLine("\t<ruleref special=\"GARBAGE\" />");
		output.AppendLine("\t<one-of>");

		foreach (var item in entries) {

			// output.Append("<mstts:express-as style=\"" + item.VoiceStyle + "\">");

			output.AppendLine("\t\t<item>" + item + "</item>");

			// output.AppendFormat("<prosody rate=\"{0}%\" pitch=\"{1}%\">", item.Rate, item.Pitch);
			// output.Append(item.Text);
			// output.Append("</prosody></mstts:express-as></voice>");
		}

		output.AppendLine("\t</one-of>");
		output.AppendLine("\t<ruleref special=\"GARBAGE\" />");
		output.AppendLine("</rule>");
		output.AppendLine("</grammar>");

		/*
		<rule id="rootRule">
			<ruleref special="GARBAGE" />
			<one-of>
			<item> adam </item>
			<item> david </item>
			<item> kevin </item>
			<item> pizza </item>
			<item> pineapple pizza </item>
			<item> microwave </item>
			</one-of>
			<ruleref special="GARBAGE" />
		</rule>
		*/


		var outString = output.ToString();

		Debug.Log("generated ssml xml for text-to-speech: " + outString);

		return outString;
	}
}
