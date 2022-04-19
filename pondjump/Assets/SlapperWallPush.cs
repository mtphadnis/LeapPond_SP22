using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapperWallPush : MonoBehaviour
{
    public bool painGrab;
    public bool caught;
    Vector3 contactPoint;
    bool inContact;
    GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<Rigidbody>().isKinematic && other.tag == "Player")
        {
            inContact = true;
            player = other.gameObject;
        }
        else if (other.gameObject.GetComponent<CharacterController>().enabled && other.tag == "Player")
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
            contactPoint = other.transform.position - transform.position;
            inContact = true;
            player = other.gameObject;
        }
    }

    private void FixedUpdate()
    {
        if (inContact && painGrab && caught)
        {
            player.transform.position = transform.position + contactPoint;
            Debug.Log("player.transform.position: " + player.transform.position + ", transform.position: " + transform.position + ", contactPoint" + contactPoint);
            player.GetComponent<CheckpointDeathManager>().Damage += 0.1f;
        }
        else if (inContact && caught)
        {
            player.transform.position = transform.position + contactPoint;
            Debug.Log("player.transform.position: " + player.transform.position + ", transform.position: " + transform.position + ", contactPoint" + contactPoint);
        }

        if (player.GetComponent<CheckpointDeathManager>().Damage >= 1 || !caught)
        {
            inContact = false;
        }
    }
}
