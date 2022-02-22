
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using StarterAssets;

public class pausemenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    public PlayerInput playerinput;
    public StarterAssetsInputs starterAssets;
    public GameObject crosshairCanvas;
    private bool isPaused = false;
    float LastPressTime = 0;
    float PressDelay = 0.5f;

    public void PauseActions()
    {
        if(starterAssets.pause)
            {
            
            if (!isPaused && LastPressTime > PressDelay)
            {
                Time.timeScale = 0;
                pauseCanvas.SetActive(true);
                crosshairCanvas.SetActive(false);
                playerinput.SwitchCurrentActionMap("UI");
                Debug.Log("Game Paused");
                isPaused = true;
                LastPressTime = 0;
            }

            else if (isPaused && LastPressTime > PressDelay)
            {
                Time.timeScale = 1;
                pauseCanvas.SetActive(false);
                crosshairCanvas.SetActive(true);
                playerinput.SwitchCurrentActionMap("Player");
                //InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
                Debug.Log("Game Unpaused");
                isPaused = false;
              

            }
        
        }
        if (LastPressTime < PressDelay)
        {
            LastPressTime += Time.deltaTime;
        }


        Debug.Log("On Button Press");
        Debug.Log("Last Press Time " + LastPressTime);
    }


 private void Update()
    {
        PauseActions();
    }


}


