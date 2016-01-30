using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyBaseState { IDLE, PATROL, OFFENSE, DEFENSE }

public class EnemyBase : MonoBehaviour
{
    
    public EnemyBaseState CurrentState = EnemyBaseState.IDLE;
    public GameObject WaypointCollection;
    public float WaypointThreshold = 0.5f;

    private NavMeshAgent agent;
    private EnemyHead head;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        head = GetComponent<EnemyHead>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case EnemyBaseState.IDLE:
                break;

            case EnemyBaseState.PATROL:
                this.updatePatrol();
                if (head != null)
                {
                    head.LookForPlayer();
                }
                break;

            case EnemyBaseState.OFFENSE:
                break;

            case EnemyBaseState.DEFENSE:
                break;

            default:
                throw new System.Exception("Current State is not set");
        }
    }

    private void updatePatrol()
    {
        float distanceRemaining = Vector3.Magnitude(agent.destination - this.transform.position) - agent.radius;

        if (distanceRemaining <= WaypointThreshold)
        {
            agent.destination = this.findRandomTargetPosition();
        }

        Component body = this.GetComponentInChildren<BoxCollider>();
        Debug.DrawLine(body.transform.position, agent.destination, new Color(0.75f, 1.0f, 0.5f, 1.0f));
        Debug.DrawRay(body.transform.position, this.transform.forward * 100, new Color(0.75f, 0.5f, 1.0f, 1.0f));
    }

    private Vector3 findRandomTargetPosition()
    {

        if (WaypointCollection == null)
            throw new System.Exception("No waypoint collection provided");

        List<Transform> waypointList = new List<Transform>();
        foreach (Transform child in WaypointCollection.transform.GetComponentsInChildren<Transform>())
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
}
