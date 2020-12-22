﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnCollisionScript : MonoBehaviour
{
	public string Level;

	private void OnCollisionEnter(Collision other) {
		SceneManager.LoadScene(Level, LoadSceneMode.Single);
	}

}