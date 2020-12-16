using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class DictationMicListenerScript : MonoBehaviour {
	public static List<Utility.IObserver<(Vector3, string)>> SpeakEvent = new List<Utility.IObserver<(Vector3, string)>>();

	// public string[] keywords = new string[] { "pineapple", "pizza", "carpet", "adam", "kevin" };

	public ConfidenceLevel confidence = ConfidenceLevel.Medium;

	protected DictationRecognizer recognizer;
	protected string word = "asdf";

	// [SerializeField]
	// private Text hypotheses;

	// [SerializeField]
	// private Text recognitions;

	private void Start() {
		recognizer = new DictationRecognizer();
		// recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;

		recognizer.DictationResult += (text, confidence) => {
			Debug.LogFormat("Dictation result: {0}", text);
			// recognitions.text += text + "\n";
		};

		recognizer.DictationHypothesis += (text) => {
			Debug.LogFormat("Dictation hypothesis: {0}", text);
			// hypotheses.text += text;
		};

		recognizer.DictationComplete += (completionCause) => {
			if (completionCause != DictationCompletionCause.Complete)
				Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
		};

		recognizer.DictationError += (error, hresult) => {
			Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
		};


		recognizer.Start();
		Debug.Log("Recognizer status: " + recognizer.Status);

		foreach (var device in Microphone.devices) {
			Debug.Log("Name: " + device);
		}
	}

	private void OnDestroy() {
		if (recognizer != null) {
			// recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
			recognizer.Stop();
			recognizer.Dispose();
		}
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		word = args.text;
		Utility.NotifyObservers(SpeakEvent, (transform.position, word));
		Debug.Log("You said: " + word);
	}
}
