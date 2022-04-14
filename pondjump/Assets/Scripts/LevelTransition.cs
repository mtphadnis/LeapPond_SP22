using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public int NextLevelIndex;
    Scene NextLevel, CurrentScene;

    private void Start()
    {
        NextLevel = SceneManager.GetSceneByBuildIndex(NextLevelIndex);

        CurrentScene = SceneManager.GetActiveScene();
        Debug.Log("CurrentScene: " + CurrentScene.name + ", " + CurrentScene.buildIndex);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.LoadScene(NextLevelIndex);
        }
    }
}
