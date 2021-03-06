using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ChangeSceneButton : MonoBehaviour
{
	public Slider mousey;
	public float valSlide;
    private void Start()
    {
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			Debug.Log("Scene " + i + ": " + SceneManager.GetSceneByBuildIndex(i).name);
		}
		
		mousey.onValueChanged.AddListener(delegate { ChangeSlideValue(); });
	}

    private void ChangeSlideValue()
    {
		valSlide = mousey.value;
	}

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
	public AudioSource playselect;
	public AudioClip scrollbeep;
	public AudioClip selectbeep;

}
