using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class resetScene : MonoBehaviour
{
	public void doAReset(string sceneName)
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		DontDestroyOnLoad(this.gameObject);


	}

}