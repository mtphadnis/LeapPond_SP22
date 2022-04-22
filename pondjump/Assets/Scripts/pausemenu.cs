
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class pausemenu : MonoBehaviour
{

    public static bool paused = false;
    public GameObject PauseCanvas;
    public GameObject AudioManager;
    public GameObject AimCanvas;
    FirstPersonControls action;

    private void Awake()
    {
        action = new FirstPersonControls();

        PauseCanvas = GameObject.Find("PauseMenu");
        AudioManager = GameObject.Find("AudioManager 1");
        AimCanvas = GameObject.Find("CrossHairBase");
    }


    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    public void Start()
    {
        Resume();
        action.Player.Paused.performed += _ => DeterminePause();
    }

    private void DeterminePause()
    {
        if (paused)
        {
            Resume();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        AudioManager.SetActive(false);
        paused = true;
        AimCanvas.SetActive(false);
        PauseCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None; 
        
    }



    public void Resume()
    {

        Time.timeScale = 1;
        AudioListener.pause = false;
        AudioManager.SetActive(true);
        paused = false;
        PauseCanvas.SetActive(false);
        AimCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

}

//EDITEDITEDIT


