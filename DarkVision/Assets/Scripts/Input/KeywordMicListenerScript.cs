using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordMicListenerScript : MonoBehaviour {

	public static List<Utility.IObserver<(Vector3, string)>> SpeakEvent = new List<Utility.IObserver<(Vector3, string)>>();

	// IDEA: Additional phrase registry system with bool layer for keywords which will stop recognised words from being broadcasted if not enabled yet
	public string[] keywords = new string[] { "pineapple", "pizza", "carpet", "adam", "kevin" };

	public ConfidenceLevel confidence = ConfidenceLevel.Medium;

	protected PhraseRecognizer recognizer;
	protected string word = "asdf";

	private void Start() {
		if (keywords != null) {
			recognizer = new KeywordRecognizer(keywords, confidence);
			recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
			
			recognizer.Start();
			Debug.Log(recognizer.IsRunning);
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
		}
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		word = args.text;
		// results.text = "You said: <b>" + word + "</b>";
		Utility.NotifyObservers(SpeakEvent, (transform.position, word));
		Debug.Log("You said: " + word);
	}
	
}
