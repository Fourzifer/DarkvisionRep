using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class NPCAnalytics : MonoBehaviour
{
    bool AllowCountingTime;
    float Timer;

    // Start is called before the first frame update
    void Start()
    {
     if(AllowCountingTime){
         Timer = Time.deltaTime;
     }   
    }

    // Update is called once per frame
    void Update()
    { 
        if(AllowCountingTime){
            Timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other){
        
        if(other.gameObject.tag == "Analytics"){
            AllowCountingTime = true;
        }
    }

    private void OnTriggerExit(Collider other){
        
        if(other.gameObject.tag == "Analytics"){
            AllowCountingTime = false;
            string VentName = transform.parent.name;

            AnalyticsEvent.Custom("Talking to " + VentName, 
                new Dictionary<string, object>
                {
                    {"Time", Mathf.RoundToInt(Timer)}
                }
            );
        }
    }

}
