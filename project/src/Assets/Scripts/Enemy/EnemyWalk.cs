using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWalk : MonoBehaviour
{
    public enum State { DISABLE_NAVMESH, IDLE, WAYPOINT_RANDOM, WAYPOINT_ORDERED, RANDOM, FOLLOW_TARGET, ESCAPE_TARGET }

    // Public Parameters
    public State state = State.IDLE;
    public GameObject waypointCollection;
    public int currentWaypointIndex = 0; //The waypoint we're currently heading towards
    public float nextDestinationThreshold = 1.0f; //how close we need to be to our destination before we pick the next destination
    public float targetPositionUpdateCooldown = 0.5f; //The time we must wait before we can update our target's position
    public float randomWalkMinDistance = 15f; //The minimum distance travelled before another random point is selected
    public float randomWalkMaxDistance = 75f; //The maximum distance travelled before another random point is selected
    public Transform lookAtTarget; //Our follow or escape target
    public float followDistance = 5f; //How close we want to be to our target
    public float escapeSafeDistance = 20f; //How far away we want to be from our target


    // Private parameters
    private float targetPositionLastUpdateTime = 0f;
    private float onNavMeshThreshold = 1.0f;

    // Private References
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            switch (state)
            {
                case State.IDLE:
                    updateIdle();
                    break;

                case State.WAYPOINT_RANDOM:
                    updateWaypointRandom();
                    break;

                case State.WAYPOINT_ORDERED:
                    updateWaypointOrdered();
                    break;

                case State.RANDOM:
                    updateRandom();
                    break;

                case State.FOLLOW_TARGET:
                    updateFollowTarget();
                    break;

                case State.ESCAPE_TARGET:
                    updateEscapeTarget();
                    break;
            }

            Debug.DrawLine(this.transform.position, agent.destination, new Color(0.75f, 1.0f, 0.5f, 1.0f));
            Debug.DrawRay(this.transform.position, this.transform.forward * 100, new Color(0.75f, 0.5f, 1.0f, 1.0f));
        }

        updateNavMeshAgent();
    }

    private void updateIdle()
    {
        agent.SetDestination(this.transform.position);
        if (agent.isOnNavMesh)
            agent.Stop();
    }

    private void updateWaypointRandom()
    {
        if (agent.remainingDistance <= nextDestinationThreshold)
            agent.SetDestination(this.getRandomWaypointPosition());
    }

    private void updateWaypointOrdered()
    {
        if (agent.remainingDistance <= nextDestinationThreshold)
            agent.SetDestination(this.getNextWaypointPosition());
    }

    private void updateRandom()
    {
        if (agent.remainingDistance <= nextDestinationThreshold)
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            Vector3 direction = new Vector3(x, 0f, z).normalized;
            float walkDistance = Random.Range(randomWalkMinDistance, randomWalkMaxDistance);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(this.transform.position + direction * walkDistance, out hit, walkDistance, 1))
                agent.destination = hit.position;
        }
    }

    private void updateFollowTarget()
    {
        // Make sure we have a target
        if (lookAtTarget == null)
            return;

        // Check if we're already close to our target
        Vector3 targetVector = lookAtTarget.position - this.transform.position;
        if (targetVector.magnitude < followDistance)
        {
            agent.SetDestination(this.transform.position);
            return;
        }

        // Only update our target position when allowed
        if (Time.time >= targetPositionLastUpdateTime + targetPositionUpdateCooldown)
        {
            agent.SetDestination(lookAtTarget.position);
            targetPositionLastUpdateTime = Time.time;
        }
    }

    private void updateEscapeTarget()
    {
        // Make sure we have a target
        if (lookAtTarget == null)
            return;

        // Only update our target position when allowed
        if (Time.time < targetPositionLastUpdateTime + targetPositionUpdateCooldown)
            return;

        Vector3 targetVector = lookAtTarget.position - this.transform.position;
        if (targetVector.magnitude < escapeSafeDistance)
        {
            //Pick a random direction away from our target
            float x = Random.Range(-1f, 1f);
            Vector3 randomVector = new Vector3(x, 0f, 1f).normalized;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, randomVector);
            Vector3 direction = rotation * -targetVector.normalized;
            float walkDistance = Random.Range(randomWalkMinDistance, randomWalkMaxDistance);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(this.transform.position + direction * walkDistance, out hit, walkDistance, 1))
            {
                agent.destination = hit.position;
                targetPositionLastUpdateTime = Time.time;
            }
                
        }
    }

    private void updateNavMeshAgent()
    {
        switch (state)
        {
            case State.DISABLE_NAVMESH:
                disableNavMeshAgent();
                break;
            case State.IDLE:
                enableNavMeshAgent();
                break;
            case State.WAYPOINT_RANDOM:
            case State.WAYPOINT_ORDERED:
            case State.RANDOM:
            case State.FOLLOW_TARGET:
            case State.ESCAPE_TARGET:
                enableNavMeshAgent();
                resumeNavMeshAgent();
                break;
        }
    }

    public void disableNavMeshAgent()
    {
        if (!agent.enabled)
            return;

        agent.Stop();
        agent.enabled = false;
        state = State.DISABLE_NAVMESH;
    }

    public void enableNavMeshAgent()
    {
        if (agent.enabled)
            return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.transform.position, out hit, onNavMeshThreshold, 1))
        {
            agent.enabled = true;
            agent.SetDestination(this.transform.position);

            if (state == State.DISABLE_NAVMESH)
                state = State.IDLE;
        }
    }

    private void resumeNavMeshAgent()
    {
        if (agent.enabled && agent.isOnNavMesh)
            agent.Resume();
    }

    private Vector3 getNextWaypointPosition()
    {
        if (waypointCollection == null)
            return this.transform.position;

        Transform[] waypointList = waypointCollection.transform.GetComponentsInChildren<Transform>();

        if (waypointList.Length == 0)
            return this.transform.position;

        this.currentWaypointIndex = (this.currentWaypointIndex + 1) % waypointList.Length;

        return waypointList[this.currentWaypointIndex].position;
    }

    private Vector3 getRandomWaypointPosition()
    {
        if (waypointCollection == null)
            return this.transform.position;
        
        List<Transform> waypointList = new List<Transform>();
        foreach (Transform child in waypointCollection.transform.GetComponentsInChildren<Transform>())
        {
            if (child.tag.Equals("Waypoint") && !child.transform.position.Equals(agent.destination))
            {
                waypointList.Add(child);
            }
        }

        if (waypointList.Count == 0)
            throw new System.Exception("No waypoints found in waypoint collection");

        int randomIndex = Random.Range(0, waypointList.Count);
        return waypointList[randomIndex].position;
    }

    public void SetWalkSpeed(float speed, float acceleration)
    {
        agent.speed = speed;
        agent.acceleration = acceleration;
    }

    public Vector3 GetVelocity()
    {
        if (agent == null)
            return Vector3.zero;

        return agent.velocity;
    }
}
