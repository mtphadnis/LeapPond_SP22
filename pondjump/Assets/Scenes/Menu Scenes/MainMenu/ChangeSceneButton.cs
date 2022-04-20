using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ChangeSceneButton : MonoBehaviour
{
	
    private void Start()
    {
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			Debug.Log("Scene " + i + ": " + SceneManager.GetSceneByBuildIndex(i).name);
		}
		
		
	}

   

    public void OnPlaySound()
	{
		FindObjectOfType<AudioManager>().Play("PlayMM");
	}

	public void PressButton()
    {
		FindObjectOfType<AudioManager>().Play("ButtonPressGeneral");
	}

	public void GoBack()
    {
		FindObjectOfType<AudioManager>().Play("LillyClick");
    }

	public void HSMenu()
    {
		
		
			FindObjectOfType<AudioManager>().Play("HSClick");
	
	}

	public void Quit()
	{
		FindObjectOfType<AudioManager>().Play("Quit");
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
