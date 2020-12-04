﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
using UnityEngine.Events;

public class MicTestMenuScript : MonoBehaviour {

	[Serializable]
	public class TestEntry {
		public string Text;
		public AudioClip Clip;
		public string Keyword;
		public UnityEvent Events;
	}

	public TestEntry[] Entries;
	private int currentEntry = 0;

	public TMP_Text text;
	public TMP_Text microphones;
	public AudioSource audioPlayer;

	public AudioClip loadingClip;

	public string nextSceneName = "VentCrawlerScene";

	public string[] keywords = new string[] { "yes", "no", "pizza" };
	public ConfidenceLevel confidence = ConfidenceLevel.Medium;
	protected PhraseRecognizer recognizer;
	protected string word = "asdf";

	private bool loading = false;

	private int micsFound = 0;

	void Start() {
		FMODUnity.RuntimeManager.WaitForAllLoads();
		// audioPlayer.PlayOneShot(WelcomeClip);

		if (keywords != null) {
			recognizer = new KeywordRecognizer(keywords, confidence);
			recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
			recognizer.Start();
			Debug.Log("Is speech to text running?: " + recognizer.IsRunning);
		}

		string mics = "Microphones found:\n\n";
		foreach (var device in Microphone.devices) {
			micsFound++;
			// Debug.Log("Name: " + device);
			mics += "\t" + device + "\n";
		}
		microphones.text = mics;

		audioPlayer.Stop();
		if (micsFound < 1) {
			StartEntry(0);
		} else if (micsFound > 1) {
			StartEntry(1);
		} else {
			StartEntry(2);
		}
	}

	// private void OnApplicationQuit() {
	private void OnDestroy() {
		if (recognizer != null && recognizer.IsRunning) {
			recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
			recognizer.Stop();
		}
	}


	void Update() {

		if (loading) {

			if (FMODUnity.RuntimeManager.HasBankLoaded("Master")) {
				Debug.Log("Master Bank Loaded");
				SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
			} else if (!FMODUnity.RuntimeManager.AnyBankLoading()) {
				Debug.LogWarning("No banks are being loaded!");
				FMODUnity.RuntimeManager.LoadBank("Master");

			}

		} else {




		}

	}

	public void StartLoading() {
		loading = true;
		text.text = "Loading...";
		audioPlayer.Stop();
		audioPlayer.PlayOneShot(loadingClip);
	}

	public void StartEntry(int index) {
		if (index > Entries.Length)
			return;
		currentEntry = index;

		text.text = Entries[index].Text;
		audioPlayer.Stop();
		if (Entries[index].Clip)
			audioPlayer.PlayOneShot(Entries[index].Clip);
	}

	public void StartNextEntry() {
		currentEntry++;
		if (currentEntry >= Entries.Length) {
			return;
		}
		StartEntry(currentEntry);
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args) {
		if (loading || micsFound < 1) {
			Debug.LogWarning("loading or no microphones! " + micsFound);
			return;
		}

		word = args.text;
		// results.text = "You said: <b>" + word + "</b>";
		// Utility.NotifyObservers(SpeakEvent, (transform.position, word));
		Debug.Log("You said: " + word);

		if (word == Entries[currentEntry].Keyword) {
			Entries[currentEntry].Events.Invoke();
		}
	}


}