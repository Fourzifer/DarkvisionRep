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
    public static int MicrophoneNr;

    // Start is called before the first frame update
    void Start()
    {
        Timer += Time.deltaTime;   
    }

    // Update is called once per frame
    void Update()
    {
        AnalyticsEvent.Custom("Player Data", 
            new Dictionary<string, object>
            {
                // {"Time", Mathf.RoundToInt(Timer)},
                {"Dialogues", DialogueNr},
                {"Levels Finished", LevelNr},
                {"Microphones Found", MicrophoneNr}
            }
        );
    }
}
