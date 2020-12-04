using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MicListenerScript : MonoBehaviour {

	public static List<Utility.IObserver<(Vector3, string)>> SpeakEvent = new List<Utility.IObserver<(Vector3, string)>>();

	// IDEA: Additional phrase registry system with bool layer for keywords which will stop recognised words from being broadcasted if not enabled yet
	public string[] keywords = new string[] { "pineapple", "pizza", "john", "carpet" };

	public ConfidenceLevel confidence = ConfidenceLevel.Medium;
	// public float speed = 1;

	// public Text results;
	// public Image target;

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

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		word = args.text;
		// results.text = "You said: <b>" + word + "</b>";
		Utility.NotifyObservers(SpeakEvent, (transform.position, word));
		Debug.Log("You said: " + word);
	}

	private void Update() {
		// var x = target.transform.position.x;
		// var y = target.transform.position.y;

        /*
		switch (word) {
			case "up":
				y += speed;
				break;
			case "down":
				y -= speed;
				break;
			case "left":
				x -= speed;
				break;
			case "right":
				x += speed;
				break;
		}

		target.transform.position = new Vector3(x, y, 0);
        */
	}

	private void OnApplicationQuit() {
		if (recognizer != null && recognizer.IsRunning) {
			recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
			recognizer.Stop();
		}
	}
}
