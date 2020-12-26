using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupHandlerScript : MonoBehaviour {
	private static PopupHandlerScript mainInstance;

	[Serializable]
	public class PopupEntry {
		public string Key;
		public GameObject Popup;
		public AudioClip Sound;
	}

	[Serializable]
	public class TimedPopupEntry : PopupEntry {
		public float Time;
		[HideInInspector]
		public float Timer;
	}

	// public AudioSource Narrator;
	public TMP_Text Timer;

	public List<PopupEntry> Popups;
	public List<TimedPopupEntry> TimedPopups;

	public TMP_Text CustomPopup;
	private float customPopupTimer = -1;

	// IDEA: draw timer for timed popups
	// TODO: play narrator voice for timed popups

	private void Awake() {
		mainInstance = this;
	}

	void Start() {
		mainInstance = this;

		// audioData = GetComponent<AudioSource>();

		foreach (var popup in mainInstance.Popups) {
			popup.Popup.SetActive(false);
		}

		Timer.gameObject.SetActive(false);
		CustomPopup?.gameObject.SetActive(false);
	}

	private void OnDestroy() {
		mainInstance = null;
	}

	private void Update() {

		bool timerIsVisible = Timer.gameObject.activeSelf;
		bool timerShouldBeVisible = false;

		if (customPopupTimer >= 0) {
			customPopupTimer -= Time.deltaTime;
			if (customPopupTimer < 0) {
				CustomPopup.gameObject.SetActive(false);
			}
		}

		foreach (var timedPopup in mainInstance.TimedPopups) {
			if (timedPopup.Popup.activeSelf) {
				timerShouldBeVisible = true;
				timedPopup.Timer -= Time.deltaTime;

				if (timedPopup.Timer < 0) {
					timedPopup.Popup.SetActive(false);
					// continue;
					break;
				}

				timerShouldBeVisible = true;
				Timer.text = timedPopup.Timer.ToString("N2");
				break;
			}
		}

		if (timerIsVisible && !timerShouldBeVisible) {
			Timer.gameObject.SetActive(false);
		} else if (!timerIsVisible && timerShouldBeVisible) {
			Timer.gameObject.SetActive(true);
		}

	}

	public static void ShowPopup(string Key) {
		if (mainInstance == null)
			return;

		//IDEA: reposition popups to fit all on screen without overlapping

		bool clipPlayed = false;

		foreach (var timedPopup in mainInstance.TimedPopups) {
			if (timedPopup.Key == Key) {
				HideTimedPopups();
				timedPopup.Popup.SetActive(true);
				timedPopup.Timer = timedPopup.Time;
				if (!clipPlayed && timedPopup.Sound) {
					// mainInstance.Narrator?.Stop();
					// mainInstance.Narrator?.PlayOneShot(timedPopup.Sound);
					PlayerCharacterScript.PlayClip(timedPopup.Sound);
					clipPlayed = true;
				}
			}
		}

		foreach (var popup in mainInstance.Popups) {
			popup.Popup.SetActive(popup.Key == Key);
			if (!clipPlayed && popup.Sound) {
				// mainInstance.Narrator?.Stop();
				// mainInstance.Narrator?.PlayOneShot(popup.Sound);
				PlayerCharacterScript.PlayClip(popup.Sound);
				clipPlayed = true;
			}
		}

	}

	public static void HidePopup(string Key) {
		if (mainInstance == null)
			return;

		//IDEA: reposition popups to fit all on screen without overlapping

		foreach (var popup in mainInstance.Popups) {
			if (popup.Key == Key)
				popup.Popup.SetActive(false);
		}

	}

	public static void HideTimedPopups() {
		if (mainInstance == null)
			return;

		//IDEA: reposition popups to fit all on screen without overlapping

		foreach (var popup in mainInstance.TimedPopups) {
			popup.Popup.SetActive(false);
		}

		mainInstance.CustomPopup.gameObject.SetActive(false);

	}

	public void EnableCustomPopup(string message, float time = 5) {
		if (!CustomPopup)
			return;

		CustomPopup.text = message;
		CustomPopup.gameObject.SetActive(true);
		customPopupTimer = time;
	}

	public static void ShowCustomPopup(string message, float time = 5) {
		mainInstance.EnableCustomPopup(message, time);
	}

}
