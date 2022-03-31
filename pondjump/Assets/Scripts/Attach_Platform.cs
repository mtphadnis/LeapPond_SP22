using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach_Platform : MonoBehaviour
{
    public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player)
        {
            Player.transform.parent = transform;

            //sets players parent to the moving platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;

            //removes the platform parent from the player
        }
    }

}
