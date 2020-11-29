using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavToFMod : MonoBehaviour {

	public string WavFile = "file";

	void Start() {

		string path = Application.dataPath + "/SFX/" + WavFile + ".wav";

		// FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
		// // TODO: read file and populate exinfo

		// FMOD.RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(
		// 	path,//Application.dataPath + "/SFX/file.wav", 
		// 	FMOD.MODE._3D, //| FMOD.MODE., 
		// 	ref exinfo,
		// 	out FMOD.Sound sound
		// );



		var system = FMODUnity.RuntimeManager.CoreSystem;

		FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
		exinfo.numchannels = 2;
		exinfo.defaultfrequency = 44100;
		exinfo.decodebuffersize = 44100;
		exinfo.length = (uint)(exinfo.defaultfrequency * exinfo.numchannels * 2 * 5); // 2 == sizeof(short) or Int16 or two bytes per sample in channel
		exinfo.format = FMOD.SOUND_FORMAT.NONE;//.PCM16;
		// exinfo.pcmreadcallback = this.pcmreadcallback;
		// exinfo.pcmsetposcallback = this.pcmsetposcallback;

		FMOD.RESULT result = system.createSound(path, FMOD.MODE.OPENUSER | FMOD.MODE.CREATESTREAM, ref exinfo, out FMOD.Sound sound);
		// result = system.playSound(sound, null, false, out FMOD.Channel channel);



	}

}
