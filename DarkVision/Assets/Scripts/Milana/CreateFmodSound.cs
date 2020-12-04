#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFmodSound : MonoBehaviour
{
    [SerializeField]
	[FMODUnity.EventRef] string sound;
	FMOD.Studio.EventInstance soundEvent;

    public KeyCode pressForSound;
    
    // Start is called before the first frame update
    void Start()
    {
        soundEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
		
        
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());
        Play();
    }

    void Play()
    {
        if (Input.GetKey(pressForSound))
          {
              FMOD.Studio.PLAYBACK_STATE fmodPbState;
              soundEvent.getPlaybackState(out fmodPbState);
              if (fmodPbState != FMOD.Studio.PLAYBACK_STATE.PLAYING) 
              {
                    soundEvent.start();
              }
          }
          if (Input.GetKeyUp (pressForSound)) 
          {
              soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
          }
    }
}
