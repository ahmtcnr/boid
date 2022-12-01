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
    [SerializeField] private float _boundMultiplier;

    [SerializeField] private Boid _boidPrefab;
    [SerializeField] private float _collisionDetectionRange;

    [SerializeField] private float _velocityLimit;


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
        Vector3 v1, v2, v3, bound;

        foreach (var boid in _flock)
        {
            v1 = Rule1(boid);
            v2 = Rule2(boid);
            v3 = Rule3(boid);
            bound = BoundBoid(boid);
            boid.Velocity = v1  +v2 + v3 + bound;
            LimitVelocity(boid);

            boid.transform.position += boid.Velocity * Time.deltaTime;
        }
    }

    private void LimitVelocity(Boid boid)
    {
        Vector3 r = Vector3.zero;

        if (boid.Velocity.magnitude > _velocityLimit)
        {
            boid.Velocity = (boid.Velocity / boid.Velocity.magnitude) * _velocityLimit;
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

        r /= _flock.Count - 1;

        return (r - boid.transform.position) / 100;
    }

    private Vector3 Rule2(Boid boid)
    {
        var r = Vector3.zero;
        foreach (var b in _flock)
        {
            if (b != boid)
            {
                if ((b.transform.position - boid.transform.position).magnitude < _collisionDetectionRange)
                {
                    r -= (b.transform.position - boid.transform.position);
                }
            }
        }

        return r;
    }

    private Vector3 Rule3(Boid boid)
    {
        var r = Vector3.zero;

        foreach (var b in _flock)
        {
            if (b != boid)
            {
                r += b.Velocity;
            }
        }

        r /= (_flock.Count - 1);

        return r;
    }

    private Vector3 BoundBoid(Boid boid)
    {
        Vector3 v = Vector3.zero;
        if (boid.transform.position.x > _dimension.x * -1)
        {
            v.x += -10;
        }
        else if (boid.transform.position.x < _dimension.x)
        {
            v.x += 10;
        }

        if (boid.transform.position.y > _dimension.y * -1)
        {
            v.y += -10;
        }
        else if (boid.transform.position.y < _dimension.y)
        {
            v.y += 10;
        }

        if (boid.transform.position.z > _dimension.z * -1)
        {
            v.z += -10;
        }
        else if (boid.transform.position.z < _dimension.z)
        {
            v.z += 10;
        }

        return v;
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

    private Vector3 CenterOfMasssss()
    {
        Vector3 boidPositionSum = Vector3.zero;
        foreach (var boid in _flock)
        {
            boidPositionSum += boid.transform.position;
        }

        //For Perceived Centre
        //return boidPositionSum / (_flock.Count - 1);

        return boidPositionSum / _flock.Count;
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
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(CenterOfMasssss(), 0.5f);
    }
}