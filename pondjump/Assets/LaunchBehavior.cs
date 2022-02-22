using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchBehavior : MonoBehaviour
{
    Transform StuckToo;
    Vector3 StartingPoint, _LaunchStrength;
    public GameObject Catch;
    GameObject Player;
    public Vector3 WalkMultiplier, SprintMultiplier;
    public float AddHeightMultiplier;
    public float LaunchClampMin;
    public float LaunchClampMax;
    float timer;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        _LaunchStrength = Player.GetComponent<thirdSoul>().LaunchStrength;
    }

    void FixedUpdate()
    {
        timer++;
        transform.position = StuckToo.position + StartingPoint;

        if(Catch != null)
        {
            Debug.DrawLine(transform.position, Catch.transform.position, Color.yellow);
            Debug.DrawLine(transform.position, Catch.transform.position + (Vector3.up * ((Catch.transform.position.y - transform.position.y) * AddHeightMultiplier)), Color.green);
        }
        
    }

    public void StickTo(Transform surface)
    {
        StuckToo = surface;
        StartingPoint = transform.position - surface.position;
    }

    public void NewCatch(GameObject newCatch)
    {
        Catch = newCatch;
    }

    public GameObject GetCatch()
    {
        return Catch;
    }


    private void OnTriggerStay(Collider other)
    {
        if (Catch != null && timer >= 20)
        {
            if (other.tag == "Player")
            {
                Vector3 VelocityMultiplier = (other.GetComponent<Rigidbody>().velocity.magnitude > 16 || other.GetComponent<CharacterController>().velocity.magnitude > 16) ? SprintMultiplier : WalkMultiplier;
                Vector3 TargetHeight = (other.GetComponent<Rigidbody>().velocity.magnitude > 16 || other.GetComponent<CharacterController>().velocity.magnitude > 16) ? Vector3.zero : Vector3.up * ((Catch.transform.position.y - transform.position.y) * AddHeightMultiplier);
                //Debug.Log("Rigid Velocity: " + other.GetComponent<Rigidbody>().velocity.magnitude + " Controller Velocity: " + other.GetComponent<CharacterController>().velocity.magnitude);

                timer = 0;
                Player.GetComponent<thirdSoul>().LaunchStart();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<Rigidbody>().AddForce(Vector3.Scale(Catch.transform.position - transform.position, VelocityMultiplier * Mathf.Clamp(Vector3.Distance(Catch.transform.position, transform.position), LaunchClampMin, LaunchClampMax)));
                //other.GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(Vector3.Scale(Catch.transform.position - transform.position, Vector3.Scale(_LaunchStrength, VelocityMultiplier) * Vector3.Distance(Catch.transform.position, transform.position)), LaunchClamp));
                //other.GetComponent<Rigidbody>().AddForce(Vector3.Scale(Catch.transform.position - transform.position, Vector3.Scale(_LaunchStrength, VelocityMultiplier) * Vector3.Distance(Catch.transform.position, transform.position)));
            }
            else
            {
                timer = 0;
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<Rigidbody>().AddForce(Vector3.Scale(Catch.transform.position - transform.position, _LaunchStrength * Vector3.Distance(Catch.transform.position, transform.position)));
            }
        }
    }
}
