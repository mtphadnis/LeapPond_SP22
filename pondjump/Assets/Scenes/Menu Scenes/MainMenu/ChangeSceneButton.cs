using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ChangeSceneButton : MonoBehaviour
{
	AudioManager audioManager;

    private void Start()
    {
		audioManager = FindObjectOfType<AudioManager>();
		audioManager.Play("Theme1");

		/*
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			Debug.Log("Scene " + i + ": " + SceneManager.GetSceneByBuildIndex(i).name);
		}
		*/
	}

   

    public void OnPlaySound()
	{
		audioManager.Play("PlayMM");
	}

	public void PressButtonSound()
    {
		audioManager.Play("ButtonPressGeneral");
	}

	public void GoBackSound()
    {
		audioManager.Play("LillyClick");
    }

	public void HSMenuSound()
    {


		audioManager.Play("HSClick");
	
	}

	public void QuitSound()
	{
		audioManager.Play("Quit");
	}


	public void LoadScene(int sceneName)
	{
		
		change = sceneName;
		Invoke("ChangeScene", 2f);
		Debug.Log(change);

		DontDestroyOnLoad(this.gameObject);
	}


	public void ChangeScene()
    {

		SceneManager.LoadScene(change);
		
	}

	public void RestartScene()
    {
		Scene thisscene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(thisscene.name);
		DontDestroyOnLoad(this.gameObject);
    }


	public int change;
	

}
