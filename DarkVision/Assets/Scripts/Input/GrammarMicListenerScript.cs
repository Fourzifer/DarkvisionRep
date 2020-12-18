using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GrammarMicListenerScript : MonoBehaviour {
	public static List<Utility.IObserver<(Vector3, string)>> SpeakEvent = new List<Utility.IObserver<(Vector3, string)>>();

	// public string[] keywords = new string[] { "pineapple", "pizza", "carpet", "adam", "kevin" };

	public string SRGSFilePath = "srgs";

	public ConfidenceLevel confidence = ConfidenceLevel.Medium;

	protected GrammarRecognizer recognizer;
	protected string word = "asdf";

	private void Start() {
		string path = Application.dataPath + "/srgs/" + SRGSFilePath + ".xml";

		if (File.Exists(path)) {
			recognizer = new GrammarRecognizer(path, confidence);
			recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;

			recognizer.Start();
			Debug.Log("Grammar recognizer started: " + recognizer.IsRunning);
		} else {
			recognizer = null;
			Debug.LogError("Specified SRGS file does not exist: " + path);
		}

		foreach (var device in Microphone.devices) {
			Debug.Log("Name: " + device);
		}
	}

	// private void OnApplicationQuit() {
	private void OnDestroy() {
		if (recognizer != null) {// && recognizer.IsRunning) {
			recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
			recognizer.Stop();
			recognizer.Dispose();
			Debug.Log("Grammar recognizer disposed.");
		}
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		word = args.text;


		// Utility.NotifyObservers(SpeakEvent, (transform.position, word));
		Debug.Log("You said: \"" + word + "\". Confidence: " + args.confidence);
		foreach (var item in args.semanticMeanings) {
			Debug.LogFormat("Semantic meaning: \nkey: {0}", item.key);
			foreach (var value in item.values) {
				Debug.LogFormat("\tvalue: {0}", value);

			}
		}
	}
}
