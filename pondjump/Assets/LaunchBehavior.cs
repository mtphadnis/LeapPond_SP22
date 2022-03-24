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
    public float LaunchClampMin;
    public float LaunchClampMax;
    public float VelocityNumeratorConstant;
    public float CatchHieghtNumeratorConstant;
    float timer;
    Vector3 difference;
    public float SprintCheck;

    public float debugFloat01, debugFloat02, debugFloat03;

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
            Debug.DrawLine(transform.position, Catch.transform.position + (Vector3.up * (CatchHieghtNumeratorConstant / (Catch.transform.position.y - transform.position.y + debugFloat01)) + new Vector3(0, debugFloat03, 0)), Color.green);
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
                Vector3 VelocityMultiplier = (other.GetComponent<Rigidbody>().velocity.magnitude > SprintCheck || other.GetComponent<CharacterController>().velocity.magnitude > SprintCheck) ? SprintMultiplier : WalkMultiplier;
                Vector3 TargetHeight = (other.GetComponent<Rigidbody>().velocity.magnitude > SprintCheck || other.GetComponent<CharacterController>().velocity.magnitude > SprintCheck) ? Vector3.zero : Vector3.up * (CatchHieghtNumeratorConstant / (Catch.transform.position.y - transform.position.y + debugFloat01) + debugFloat02) + new Vector3(0,debugFloat03,0);
                Debug.Log("Rigid Velocity: " + other.GetComponent<Rigidbody>().velocity.magnitude + " Controller Velocity: " + other.GetComponent<CharacterController>().velocity.magnitude);

                timer = 0;
                Player.GetComponent<thirdSoul>().LaunchStart();
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;

                difference = Catch.transform.position - transform.position;

                other.GetComponent<Rigidbody>().AddForce(Vector3.Scale(difference + TargetHeight, VelocityMultiplier) * ((VelocityNumeratorConstant / (Vector3.Magnitude(difference) + 20)) + 3));

                Debug.Log("Difference: " + difference + " Distance: " + Vector3.Magnitude(difference) + " Multiplier: " + ((VelocityNumeratorConstant / (Vector3.Magnitude(difference) + 20)) + 3) + " Launch: " + Vector3.Scale(difference, VelocityMultiplier) * ((VelocityNumeratorConstant / (Vector3.Magnitude(difference) + 20)) + 3));
            }
            else if(other.tag != "Platform")
            {
                timer = 0;
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.GetComponent<Rigidbody>().AddForce(Vector3.Scale(Catch.transform.position - transform.position, _LaunchStrength * Vector3.Distance(Catch.transform.position, transform.position)));
            }
        }
    }
}
