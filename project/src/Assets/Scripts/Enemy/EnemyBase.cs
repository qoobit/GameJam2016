﻿using UnityEngine;
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
    private Transform body;

    // Use this for initialization
    void Start()
    {
        body = this.transform.FindChild("Body");
        agent = GetComponent<NavMeshAgent>();
        head = GetComponent<EnemyHead>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update our state if head is locked on to something
        if (head.currentState == EnemyHeadState.LOCKED)
        {
            this.CurrentState = EnemyBaseState.OFFENSE;
        }
        else if (head.currentState == EnemyHeadState.LOST)
        {
            this.CurrentState = EnemyBaseState.IDLE;
        }
        else if (head.currentState == EnemyHeadState.IDLE)
        {
            this.CurrentState = EnemyBaseState.PATROL;
        }

        //Enemy State machine 
        switch (CurrentState)
        {
            case EnemyBaseState.IDLE:
                agent.Stop();
                break;

            case EnemyBaseState.PATROL:
                agent.Resume();
                this.updatePatrol();
                if (head != null)
                {
                    head.LookForPlayer();
                }
                break;

            case EnemyBaseState.OFFENSE:
                agent.Stop();
                this.updateOffense();
                break;

            case EnemyBaseState.DEFENSE:
                break;

            default:
                throw new System.Exception("Current State is not set");
        }
    }

    private void updateOffense()
    {
        if (head == null) return;

        //Turn the body towards our target but remain upright
        Vector3 lookAtVector = head.lookAtTarget.position - new Vector3(0, head.lookAtTarget.position.y - body.position.y, 0);
        this.transform.LookAt(lookAtVector);
    }

    private void updatePatrol()
    {

        float distanceRemaining = Vector3.Magnitude(agent.destination - this.transform.position) - agent.radius;
        
        if (distanceRemaining <= WaypointThreshold)
        {
            agent.destination = this.findRandomTargetPosition();
        }

        //Component collider = this.GetComponentInChildren<BoxCollider>();
        Debug.DrawLine(this.transform.position, agent.destination, new Color(0.75f, 1.0f, 0.5f, 1.0f));
        Debug.DrawRay(this.transform.position, this.transform.forward * 100, new Color(0.75f, 0.5f, 1.0f, 1.0f));
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

    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }
}
