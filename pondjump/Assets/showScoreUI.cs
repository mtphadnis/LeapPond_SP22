using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showScoreUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject highscoreObj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void turnOnOrOff()
    {
        if (highscoreObj.activeInHierarchy)
        {
            highscoreObj.SetActive(false);
        }
        else if (!highscoreObj.activeInHierarchy)
        {
            highscoreObj.SetActive(true);
        }
    }
}
