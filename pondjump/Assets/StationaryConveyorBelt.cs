using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationaryConveyorBelt : MonoBehaviour
{
    public GameObject Player;
    public GameObject Start, Fin;
    bool sliding;
    public int dividing;

    private void FixedUpdate()
    {
        if (sliding)
        {
            Slide();
        }
        //this.GetComponent<Shader>().SetTextureOffset("Conveyor", this.GetComponent<Shader>().GetTextureOffset("Conveyor") - new Vector2(0, 0.01f));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other: " + other.name);
        if (other.gameObject == Player)
        {
            Debug.Log("Fool");
            sliding = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == Player)
        {
            Debug.Log("Coward");
            sliding = false;
        }
    }

    public void Slide()
    {
        Player.transform.position += Vector3.Normalize(Fin.transform.position - Start.transform.position) / dividing;
    }
}
