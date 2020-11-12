using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuffleParameter : MonoBehaviour {
	// Start is called before the first frame update

    [SerializeField]
	[FMODUnity.EventRef] string sound;
	FMOD.Studio.EventInstance soundEvent;
    // [SerializeField]
	// [FMODUnity.ParamRef] string parameter;

	GameObject player;
    Vector3 playerPos;
    Vector3 thisPos;

    [SerializeField]
    float maxDistance;

    FMOD.Studio.PARAMETER_ID healthParameterId;

	void Start() {
		
        soundEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
		soundEvent.start();
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent, GetComponent<Transform>(), GetComponent<Rigidbody>());

        FMOD.Studio.EventDescription healthEventDescription;
        soundEvent.getDescription(out healthEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION healthParameterDescription;
        healthEventDescription.getParameterDescriptionByName("CafeParameter", out healthParameterDescription);
        healthParameterId = healthParameterDescription.id;

        player = GameObject.FindGameObjectWithTag("Player");

	}

	// Update is called once per frame
	void Update() {
        
        if (player != null) {
			playerPos = player.transform.position;
		}

       

        if( CalcDistance(playerPos, thisPos) > maxDistance)
        {
            Debug.Log("distance is: " + CalcDistance(playerPos, thisPos));
            soundEvent.setParameterByID(healthParameterId, CalcDistance(playerPos, thisPos));
            // soundEvent.setParameterByName("CafeParameter", CalcDistance(playerPos, thisPos));
        }
	}

    float CalcDistance(Vector3 obj2, Vector3 obj1)
    {
        return Mathf.Sqrt(Mathf.Pow((obj1.x - obj2.x), 2) + Mathf.Pow((obj1.z - obj2.z), 2));
    }
}
