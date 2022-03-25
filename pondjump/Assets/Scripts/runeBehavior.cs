using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runeBehavior : MonoBehaviour
{
    Transform StuckToo;
    Vector3 StartingPoint;

    void Update()
    {
        if(StuckToo != null && StartingPoint != null)
        {
            transform.position = StuckToo.position + StartingPoint;
        }
        
    }

    public void StickTo(Transform surface)
    {
        StuckToo = surface;
        StartingPoint = transform.position - surface.position;
    }
}
