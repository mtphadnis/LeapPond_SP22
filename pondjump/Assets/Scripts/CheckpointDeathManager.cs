using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CheckpointDeathManager : MonoBehaviour
{
    [Space(10)]
    [Header("Health")]
    [Tooltip("No Death for Player")]
    public bool GodMode;
    [Tooltip("When True will launch new scene for death menu if false will enable canvas on player")]
    public bool SceneDeath;
    [Tooltip("Current Player Health")]
    [Range(0, 1)]
    public float Damage;
    //whether the player is being hurt or not
    public bool inPain;
    public Image HealthUI;
    [Tooltip("How much health percentage is lost per second while being damaged")]
    public float HealthLoss;
    [Tooltip("How much health percentage is gained per second while  not being damaged")]
    public float HealthGain;
    [Tooltip("The Death Menu Canvas")]
    public GameObject DeathMenuCanvas;
    [Tooltip("Last Checkpoint you hit")]
    public Vector3 Checkpoint;
    [Tooltip("Amount of times a player has died on a given session")]
    public static int DeathCount;

    Scene DeathMenu;

    private void Start()
    {
        Damage = 0;

        Debug.Log("Scene Count: " + SceneManager.sceneCount);
        Debug.Log("Scene Build Count: " + SceneManager.sceneCountInBuildSettings);
        for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            Debug.Log("Scene " + i + ": " + SceneManager.GetSceneByBuildIndex(i).name);
        }
        //DeathMenu = SceneManager.GetSceneByName("Death Menu");
        //HealthUI = GameObject.Find("HealthUI").GetComponent<Image>();

        Checkpoint = transform.position;
    }

    private void FixedUpdate()
    {
        Healing();

        if (Damage >= 1 && !GodMode && SceneDeath)
        {
            DeathCount++;
            Debug.Log("DeathCount: " + DeathCount);
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(1);
        }
        else if (Damage >= 1 && !GodMode && !SceneDeath)
        {
            DeathCount++;
            Debug.Log("DeathCount: " + DeathCount);
            Cursor.lockState = CursorLockMode.None;
            PauseGame();
            DeathMenuCanvas.SetActive(true);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public void Continue()
    {
        Damage = 0;
        DeathMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        transform.position = Checkpoint;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Menu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Application.Quit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            Checkpoint = other.transform.position;
            other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        if (other.transform.tag == "Deadly" && Damage < 0.33)
        {
            Damage = 0.33f;
        }
        else if (other.transform.tag == "Deadly" && Damage < 1)
        {
            Damage += HealthLoss;
            inPain = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Deadly" && Damage > 0)
        {
            inPain = false;
        }
    }

    void Healing()
    {
        Damage += !inPain && Damage > 0 ? HealthGain : 0;
        var tempColor = HealthUI.color;
        tempColor.a = Damage;
        HealthUI.color = tempColor;
        HealthUI.fillAmount = Damage;
    }
}
