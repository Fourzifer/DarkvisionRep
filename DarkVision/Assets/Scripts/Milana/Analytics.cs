using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analytics : MonoBehaviour
{
    bool AllowCountingTime;
    public static float Timer;
    public static int DialogueNr;
    public static int LevelNr;
    public static bool TestState = false;
    public static int MicrophoneNr;
    public static string LastNPC;

    void Awake()
    {
        Timer += Time.deltaTime;
    }
    // Start is called before the first frame update
    void Start()
    {  

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnApplicationQuit() {
		
        SendAnalytics();
        Debug.Log("Analytics Sent");
		
	}

    void SendAnalytics(){
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
