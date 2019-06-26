using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class RoachAI : MonoBehaviour
{
    public Transform target;
    public Path path;

    public float speed = 2;
    public float nextWaypointDistance = 3;
    public bool reachedEndOfPath;
    public float damping = 6;

    Seeker seeker;
    CharacterController controller;
    Vector3 nextWaypoint;

    int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null)
        {
            return;
        }

        reachedEndOfPath = false;
        float distanceToWaypoint;

        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }

        nextWaypoint = path.vectorPath[currentWaypoint];
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
        Vector3 dir = (nextWaypoint - transform.position).normalized;
        Vector3 velocity = dir * speed * speedFactor;
        controller.SimpleMove(velocity);
    }

    void LateUpdate()
    {
        if (path == null)
        {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(nextWaypoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);

        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
