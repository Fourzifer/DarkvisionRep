using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioPlayback : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField]
    [EventRef]
    private string SelectAudio;
    [SerializeField]
    private KeyCode pressToStopSound;
    private EventInstance Audio;
    private EventDescription AudioDes;
    private StudioListener Listener;
    private PLAYBACK_STATE pb;
    
    // Start is called before the first frame update
    void Start()
    {
        Audio = FMODUnity.RuntimeManager.CreateInstance(SelectAudio);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Audio, GetComponent<Transform>(), GetComponent<Rigidbody>());
        StartPlayback();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(pressToStopSound))
          {
            StopPlayback();
          }

          GetComponent<FMODUnity.StudioEventEmitter>().Play();
    }

    public void StopPlayback()
    {
        FMOD.Studio.PLAYBACK_STATE fmodPbState;
        Audio.getPlaybackState(out fmodPbState);
        
        if (fmodPbState != FMOD.Studio.PLAYBACK_STATE.STOPPED) 
        {
            Audio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void StartPlayback()
    {
        FMOD.Studio.PLAYBACK_STATE fmodPbState;
        Audio.getPlaybackState(out fmodPbState);
        
        if (fmodPbState != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
        {
            Audio.start();
        }
    }
}
