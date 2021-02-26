using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analytics : MonoBehaviour {
	bool RunTimer = false;
	bool AllowCountingTime;
	public static float Timer = 0;
	public static int DialogueNr = 0;
	public static int LevelNr = 0;
	public static bool TestState = false;
	public static int MicrophoneNr = 0;
	public static string LastNPC = "Unknown";

	void Awake() {
		RunTimer = true;
	}
	// Start is called before the first frame update
	void Start() {

	}

	void Update() {
		if (RunTimer) {
			Timer += Time.deltaTime;
		}

	}
	private void OnApplicationQuit() {
		RunTimer = false;
		SendAnalytics();
		Debug.Log("Analytics Sent");
		Debug.Log("Time spent playing : " + Timer);

	}

	void SendAnalytics() {
		AnalyticsEvent.Custom("In-Game Data",
			new Dictionary<string, object>
			{
				{"Minutes played", Mathf.RoundToInt(Timer/60f)},
				{"Last NPC", LastNPC},
				{"Dialogues", DialogueNr},
				{"Levels Finished", LevelNr},
				{"Microphone Test", TestState},
				{"Microphones Found", MicrophoneNr}

			}
		);
	}

}
