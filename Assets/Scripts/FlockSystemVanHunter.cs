using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class FlockSystemVanHunter : MonoBehaviour
{
    [SerializeField] private int _boidCount;

    [SerializeField] private Vector3 _dimension;
    [SerializeField] private Boid _boidPrefab;

    private List<Boid> _boids = new List<Boid>();


    [SerializeField] private float _visualRange;
    [SerializeField] private float _protectedRange;
    [SerializeField] private float _centeringFactor;
    [SerializeField] private float _matchingFactor;
    [SerializeField] private float _avoidFactor;

    [SerializeField] private float _turnFactor;


    [SerializeField] private float _minSpeed;

    [SerializeField] private float _maxSpeed;

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
            _boids.Add(spawnedBoid);
        }
    }


    private void Update()
    {
        foreach (var boid in _boids)
        {
            float closeDx = 0f, closeDy = 0f, closeDz = 0f;
            float xPosAvg = 0f, yPosAvg = 0f, zPosAvg = 0f;
            float xVelAvg = 0f, yVelAvg = 0f, zVelAvg = 0f;
            int neighbourCount = 0;
            foreach (var otherBoid in _boids)
            {
                var diff = boid.transform.position - otherBoid.transform.position;

                if (Mathf.Abs(diff.x) < _visualRange && Mathf.Abs(diff.y) < _visualRange && Mathf.Abs(diff.z) < _visualRange)
                {
                    var squaredDistance = diff.sqrMagnitude;

                    if (squaredDistance < _protectedRange)
                    {
                        closeDx += boid.transform.position.x - otherBoid.transform.position.x;
                        closeDy += boid.transform.position.y - otherBoid.transform.position.y;
                        closeDz += boid.transform.position.z - otherBoid.transform.position.z;
                    }
                    else if (squaredDistance < Mathf.Pow(_visualRange, 2))
                    {
                        xPosAvg += otherBoid.transform.position.x;
                        yPosAvg += otherBoid.transform.position.y;
                        zPosAvg += otherBoid.transform.position.z;

                        xVelAvg += otherBoid.Velocity.x;
                        yVelAvg += otherBoid.Velocity.y;
                        zVelAvg += otherBoid.Velocity.z;

                        neighbourCount++;
                    }
                }
            }

            if (neighbourCount > 0)
            {
                xPosAvg /= neighbourCount;
                yPosAvg /= neighbourCount;
                zPosAvg /= neighbourCount;

                boid.Velocity.x = (boid.Velocity.x +
                                   (xPosAvg - boid.transform.position.x) * _centeringFactor +
                                   (xVelAvg - boid.transform.position.x) * _matchingFactor);

                boid.Velocity.y = (boid.Velocity.y +
                                   (yPosAvg - boid.transform.position.y) * _centeringFactor +
                                   (yVelAvg - boid.transform.position.y) * _matchingFactor);

                boid.Velocity.z = (boid.Velocity.z +
                                   (zPosAvg - boid.transform.position.z) * _centeringFactor +
                                   (zVelAvg - boid.transform.position.z) * _matchingFactor);
            }

            boid.Velocity.x += (closeDx * _avoidFactor);
            boid.Velocity.y += (closeDy * _avoidFactor);
            boid.Velocity.z += (closeDz * _avoidFactor);

            if (boid.transform.position.x > 20)
            {
                boid.Velocity.x += _turnFactor;
            }
            else if (boid.transform.position.x < -20)
            {
                boid.Velocity.x -= _turnFactor;
            }
            
            if (boid.transform.position.y > 20)
            {
                boid.Velocity.y += _turnFactor;
            }
            else if (boid.transform.position.y < -20)
            {
                boid.Velocity.y -= _turnFactor;
            }

            if (boid.transform.position.z > 20)
            {
                boid.Velocity.z += _turnFactor;
            }
            else if (boid.transform.position.z < -20)
            {
                boid.Velocity.z -= _turnFactor;
            }

            var speed = boid.Velocity.magnitude;

            if (speed < _minSpeed)
            {
                boid.Velocity = (boid.Velocity / speed) * _minSpeed;
            }

            if (speed > _maxSpeed)
            {
                boid.Velocity = (boid.Velocity / speed) * _maxSpeed;
            }


            boid.transform.position += boid.Velocity * Time.deltaTime;
        }
    }


    private void OnDrawGizmos()
    {
        Handles.DrawWireCube(transform.position, _dimension);

        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(CenterOfMass(), 0.5f);
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(CenterOfMasssss(), 0.5f);
    }
}