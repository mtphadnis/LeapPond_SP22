using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class ChangeSceneButton : MonoBehaviour
{

	public void PlaySound()
	{
		playselect.PlayOneShot(selectbeep);
	}

	public void GoBackSound()
    {
		playselect.PlayOneShot(scrollbeep);
	}
	public void LoadScene(int sceneName)
	{
		playselect.PlayOneShot(selectbeep);
		change = sceneName;
		Invoke("ChangeScene", 2f);

	}

	public void ChangeScene()
    {

		SceneManager.LoadScene(change);

	}


	public int change;
	public AudioSource playselect;
	public AudioClip scrollbeep;
	public AudioClip selectbeep;

}
