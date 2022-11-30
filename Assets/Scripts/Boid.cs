using System;
using UnityEngine;


public class Boid : MonoBehaviour
{

    public Vector3 Velocity;


    private void Update()
    {
        transform.position += Velocity * Time.deltaTime;
    }
}