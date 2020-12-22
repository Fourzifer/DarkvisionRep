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
    public static int LastNPC;

    // Start is called before the first frame update
    void Start()
    {
        // Timer += Time.deltaTime;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SendAnalytics(){
        AnalyticsEvent.Custom("In-Game Data", 
            new Dictionary<string, object>
            {
                // {"Time", Mathf.RoundToInt(Timer)},
                // {"Last NPC", LastNPC},
                {"Dialogues", DialogueNr},
                {"Levels Finished", LevelNr},
                {"Microphone Test", TestState},
                {"Microphones Found", MicrophoneNr}
                
            }
        );
    }
}
