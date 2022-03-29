using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public string NextLevelName;
    Scene NextLevel;

    private void Start()
    {
        NextLevel = SceneManager.GetSceneByName(NextLevelName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.SetActiveScene(NextLevel);
        }
    }
}
