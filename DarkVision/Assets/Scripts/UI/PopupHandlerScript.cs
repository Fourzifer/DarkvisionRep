using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public List<PopupEntry> Popups;
	public List<TimedPopupEntry> TimedPopups;

	void Start() {
		mainInstance = this;

		foreach (var popup in mainInstance.Popups) {
			popup.Popup.SetActive(false);
		}
	}

	private void OnDestroy() {
		mainInstance = null;
	}

	private void Update() {
		foreach (var timedPopup in mainInstance.TimedPopups) {
			if (timedPopup.Popup.activeSelf) {
				timedPopup.Timer -= Time.deltaTime;
				if (timedPopup.Timer < 0)
					timedPopup.Popup.SetActive(false);
			}
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
			if (timedPopup.Key == Key){
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

}
