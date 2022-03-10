
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using StarterAssets;

public class pausemenu : MonoBehaviour
{

    public static bool paused = false;
    public GameObject PauseCanvas1;
    FirstPersonControls action;

    private void Awake()
    {
        action = new FirstPersonControls();
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
        paused = true;
        PauseCanvas1.SetActive(true);
        
    }



    public void Resume()
    {

        Time.timeScale = 1;
        paused = false;
        PauseCanvas1.SetActive(false);
    }

}


