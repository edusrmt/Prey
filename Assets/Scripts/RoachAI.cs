using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class RoachAI : MonoBehaviour
{
    public Transform player;
    public Path path;

    public float walkSpeed = 1;
    public float runSpeed = 2;
    public float visionRange = 14;
    public float timeToScare = 10;
    public float jumpDistance = 3;
    public float nextWaypointDistance = 3;
    public bool reachedEndOfPath;
    public float damping = 6;

    Seeker seeker;
    CharacterController controller;
    Animator animator;
    AudioSource audioSource;

    Transform target;
    Vector3 nextWaypoint;

    int currentWaypoint = 0;
    public float speed = 0;
    bool angry = false;
    float timeSinceVision = 0;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        speed = walkSpeed;

        GetRandomTarget();
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceVision += Time.deltaTime;

        if (path == null || animator.GetCurrentAnimatorStateInfo(0).IsName("Scream"))
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) <= visionRange)
        {
            RaycastHit hit;
            Vector3 direction = new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z);

            if (Physics.Raycast(transform.position, direction, out hit, visionRange))
            {
                SeeingPlayer(hit.transform.CompareTag("Player"));
            }
        } else
        {
            SeeingPlayer(false);
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
                    GetRandomTarget();
                    animator.SetBool("isWalking", false);
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

        Vector3 forward = new Vector3(nextWaypoint.x - transform.position.x, 0, nextWaypoint.z - transform.position.z);
        Quaternion rotation = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }

    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
            animator.SetBool("isWalking", true);
        }
    }

    void GetRandomTarget()
    {
        speed = walkSpeed;
        animator.speed = 1;
        target = MazeGenerator.instance.GetCells()[Random.Range(0, MazeGenerator.instance.GetCells().Count)];
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void SeeingPlayer (bool currentAnger)
    {
        if (animator.GetBool("scream") == true)
            animator.SetBool("scream", false);


        if (Vector3.Distance(transform.position, player.position) <= jumpDistance)
        {
            KillPlayer();
        }

        if (angry != currentAnger)
        {
            if(currentAnger)
            {
                if (timeSinceVision >= timeToScare)
                {
                    animator.SetBool("scream", true);
                    audioSource.Play();
                }

                speed = runSpeed;
                animator.speed = 2;

                target = player;
                seeker.StartPath(transform.position, target.position, OnPathComplete);
            } else
            {

            }

            angry = currentAnger;
        }

        if (currentAnger)
            timeSinceVision = 0;
    }

    public void KillPlayer ()
    {
        animator.SetBool("jump", true);
        audioSource.Play();
        GameManager.instance.GameOver();
    }
}
