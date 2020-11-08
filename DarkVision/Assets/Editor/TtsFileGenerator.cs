using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Threading.Tasks;
using System.Text;
using Microsoft.CognitiveServices.Speech;
using System;
using Microsoft.CognitiveServices.Speech.Audio;

public class TtsFileGenerator : EditorWindow {

	[Serializable]
	public class VoiceEntry {
		public enum Voice {
			USAriaNeural,
			USGuyNeural,
			// UKLibbyNeural,
			// UKMiaNeural,
		}



		public Voice Narrator = VoiceEntry.Voice.USAriaNeural;
		// [Range(-100, 200)]
		public float Rate = 0;
		// [Range(-50, 50)]
		public float Pitch = 0;
		public string VoiceStyle = "General";
		public string Text = "";

	}

	public List<VoiceEntry> Text = new List<VoiceEntry>(){
		new VoiceEntry(){
			Text = "This is placeholder text"
		}
	};

	public string ApiKey;
	public string Location = "westeurope";

	public string OutputFile = "file";

	public int pcmPosition = 0;
	public int samplerate = 44100;
	public float frequency = 440;

	[MenuItem("DarkVision/TtsFileGenerator")]
	private static void ShowWindow() {
		var window = GetWindow<TtsFileGenerator>();
		window.titleContent = new GUIContent("TtsFileGenerator");
		window.Show();
	}

	private void OnGUI() {
		ApiKey = EditorGUILayout.TextField("Api Key", ApiKey);
		Location = EditorGUILayout.TextField("Location", Location);

		// EditorGUILayout
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Voice entries");
		for (int i = 0; i < Text.Count; i++) {
			var item = Text[i];
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Entry " + (i + 1));
			if (GUILayout.Button("Remove")) {
				Text.Remove(Text[i]);
			}
			EditorGUILayout.EndHorizontal();

			// EditorGUILayout.BeginHorizontal();
			item.Narrator = (VoiceEntry.Voice)EditorGUILayout.EnumPopup("Narrator", item.Narrator);
			item.VoiceStyle = EditorGUILayout.TextField("Voice style", item.VoiceStyle);

			item.Rate = EditorGUILayout.Slider("Rate", item.Rate, -100, 200);
			item.Pitch = EditorGUILayout.Slider("Pitch", item.Pitch, -50, 50);
			// EditorGUILayout.EndHorizontal();

			item.Text = EditorGUILayout.TextArea(item.Text);
			EditorGUILayout.Space();
		}

		EditorGUILayout.Space();
		if (GUILayout.Button("Add entry")) {
			Text.Add(new VoiceEntry());
		}
		EditorGUILayout.Space();

		OutputFile = EditorGUILayout.TextField("Output file", OutputFile);

		if (GUILayout.Button("Generate File")) {
			// Task.Run(() => SynthesizeAudioAsync(ApiKey, Location, GenerateXML(), OutputFile));
			SynthesizeAudioAsync(ApiKey, Location, GenerateXML(), OutputFile);
			// var sound = AudioClip.Create("asdf", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
			// AssetDatabase.CreateAsset(sound, "Assets/SFX/asdf.asset");
		}

	}

	static async Task SynthesizeAudioAsync(string apiKey, string location, string xml, string outputFile) {
		// void SynthesizeAudioAsync(string apiKey, string location, string xml) {

		var config = SpeechConfig.FromSubscription(apiKey, location);
		string path = Application.dataPath + "/SFX/" + outputFile + ".wav";
		Debug.Log("Attempting to write sound file to path: " + path);
		// using (var audioConfig = AudioConfig.FromWavFileOutput(path)) {

		// using var synthesizer = new SpeechSynthesizer(config, audioConfig);
		// await synthesizer.SpeakTextAsync("A simple test to write to a file.");

		// using (var synthesizer = new SpeechSynthesizer(config, audioConfig)) {
		using (var synthesizer = new SpeechSynthesizer(config, null)) {
			// var result = await synthesizer.SpeakTextAsync("Getting the response as an in-memory stream.");
			var result = await synthesizer.SpeakSsmlAsync(xml);
			// synthesizer.SpeakSsmlAsync(xml);
			// using (var stream = AudioDataStream.FromResult(result)) {
			// var sound = AudioClip.Create("asdf", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
			// AssetDatabase.CreateAsset(sound, "assets/SFX/asdf.wav");

			// }
			using (var stream = AudioDataStream.FromResult(result)) {
				await stream.SaveToWaveFileAsync(path);

			}
		}



		// }
		Debug.Log("Wrote sound file to path: " + path);

	}

	void OnAudioRead(float[] data) {
		int count = 0;
		while (count < data.Length) {
			data[count] = Mathf.Sin(2 * Mathf.PI * frequency * pcmPosition / samplerate);
			pcmPosition++;
			count++;
		}
	}

	void OnAudioSetPosition(int newPosition) {
		pcmPosition = newPosition;
	}

	public string GenerateXML() {
		StringBuilder output = new StringBuilder("<speak xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\" xmlns:emo=\"http://www.w3.org/2009/10/emotionml\" version=\"1.0\" xml:lang=\"en-US\">");

		foreach (var item in Text) {
			switch (item.Narrator) {
				case VoiceEntry.Voice.USAriaNeural:
				default:
					output.Append("<voice name=\"en-US-AriaNeural\">");
					break;
				case VoiceEntry.Voice.USGuyNeural:
					output.Append("<voice name=\"en-US-GuyNeural\">");
					break;
				// case VoiceEntry.Voice.UKLibbyNeural:
				// 	output.Append("<voice name=\"en-UK-LibbyNeural\">");
				// 	break;
				// case VoiceEntry.Voice.UKMiaNeural:
				// 	output.Append("<voice name=\"en-UK-MiaNeural\">");
				// 	break;
			}


			output.Append("<mstts:express-as style=\"" + item.VoiceStyle + "\">");


			output.AppendFormat("<prosody rate=\"{0}%\" pitch=\"{1}%\">", item.Rate, item.Pitch);
			output.Append(item.Text);
			output.Append("</prosody></mstts:express-as></voice>");
		}

		output.Append("</speak>");

		var outString = output.ToString();

		Debug.Log("generated ssml xml for text-to-speech: " + outString);

		return outString;
	}
}
