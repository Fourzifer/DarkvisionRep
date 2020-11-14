using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {


	public TMP_Text text;
	public AudioSource audioPlayer;
	public AudioClip pressToPlayClip;
	public AudioClip loadingClip;

	public string nextSceneName = "VentCrawlerScene";

	bool loading = false;


	void Start() {
		// FMODUnity.RuntimeManager.WaitForAllLoads();
		audioPlayer.PlayOneShot(pressToPlayClip);
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

			if (Input.GetKey(KeyCode.Space)
			|| Input.GetButton("gamepadInteract")
			|| Input.touchCount > 0
			|| Input.GetMouseButton(0)
			) {
				loading = true;
				text.text = "Loading...";
				audioPlayer.Stop();
				audioPlayer.PlayOneShot(loadingClip);
			}

		}

	}
}
