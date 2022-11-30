using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlockSystem : MonoBehaviour
{
    [SerializeField] private int _boidCount;

    [SerializeField] private Vector3 _dimension;

    [SerializeField] private Boid _boidPrefab;


    private HashSet<Boid> _flock = new HashSet<Boid>();

    private void Awake()
    {
        CreateFlock();
    }

    private void CreateFlock()
    {
        for (int i = 0; i < _boidCount; i++)
        {
            Vector3 startPos = new Vector3(Random.Range(_dimension.x / -2, _dimension.x / 2), Random.Range(_dimension.y / -2, _dimension.y / 2),
                Random.Range(_dimension.z / -2, _dimension.z / 2));
            var spawnedBoid = Instantiate(_boidPrefab, startPos, Quaternion.identity, transform);
            _flock.Add(spawnedBoid);
        }
    }


    void Update()
    {
        MoveBoids();
    }

    private void MoveBoids()
    {
        Vector3 v1, v2, v3;

        foreach (var boid in _flock)
        {
            v1 = Rule1(boid);


            boid.Velocity = v1.normalized;

            //boid.transform.position += boid.Velocity * Time.deltaTime;

        }
    }

    private Vector3 Rule1(Boid boid)
    {


        var r = Vector3.zero;
        foreach (var b in _flock)
        {
            if (b != boid)
            {
                r += b.transform.position;
            }
        }

        return (r - boid.transform.position) / 100;
    }


    private Vector3 CenterOfMass()
    {
        Vector3 boidPositionSum = Vector3.zero;
        foreach (var boid in _flock)
        {
            boidPositionSum += boid.transform.position;
        }
        
        //For Perceived Centre
        return boidPositionSum / (_flock.Count - 1);
        
        //return boidPositionSum / _flock.Count;
    }


    // [Button("Create Flock")]
    // private void ReCreateFlock()
    // {
    //     if (!Application.isPlaying) return;
    //
    //     foreach (var boid in _flock)
    //     {
    //         Destroy(boid.gameObject);
    //     }
    //
    //     CreateFlock();
    // }


    private void OnDrawGizmos()
    {
        Handles.DrawWireCube(transform.position, _dimension);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CenterOfMass(), 0.5f);
    }
}