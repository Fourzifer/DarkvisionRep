using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookScript : MonoBehaviour {

	private static NotebookScript mainInstance = null;

	void Start() {
        mainInstance = this;
		gameObject.SetActive(false);
	}

	private void OnDestroy() {
		if (mainInstance == this) 
			mainInstance = null;

	}

	void Update() {

	}

	public static void Show(){
		mainInstance.gameObject.SetActive(true);
	}

	public static void Hide(){
		mainInstance.gameObject.SetActive(false);
	}
}
