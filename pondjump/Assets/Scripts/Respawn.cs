using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{


    public Transform spawnPoint;//Add empty gameobject as spawnPoint
    public float minHeightForDeath = 1;
    public GameObject player; //Add your player

    void Update()
    {
        if (player.transform.position.y < minHeightForDeath)
            player.transform.position = spawnPoint.position;
    }

    


}
