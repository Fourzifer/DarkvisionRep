using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookScript : MonoBehaviour {

	private static NotebookScript mainInstance = null;

	void Start() {
        mainInstance = this;
	}

	private void OnDestroy() {
		if (mainInstance == this) 
			mainInstance = null;

	}

	void Update() {

	}
}
