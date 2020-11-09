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
	}

	[Serializable]
	public class TimedPopupEntry : PopupEntry {
		public float Time;
		[HideInInspector]
		public float Timer;
	}

	public TMP_Text Timer;

	public List<PopupEntry> Popups;
	public List<TimedPopupEntry> TimedPopups;

	// IDEA: draw timer for timed popups
	// TODO: play narrator voice for timed popups

	void Start() {
		mainInstance = this;

		foreach (var popup in mainInstance.Popups) {
			popup.Popup.SetActive(false);
		}

		Timer.gameObject.SetActive(false);
	}

	private void OnDestroy() {
		mainInstance = null;
	}

	private void Update() {

		bool timerIsVisible = Timer.gameObject.activeSelf;
		bool timerShouldBeVisible = false;

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

		foreach (var popup in mainInstance.Popups) {
			popup.Popup.SetActive(popup.Key == Key);
		}

		foreach (var timedPopup in mainInstance.TimedPopups) {
			if (timedPopup.Key == Key) {
				HideTimedPopups();
				timedPopup.Popup.SetActive(true);
				timedPopup.Timer = timedPopup.Time;
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

	}

}
