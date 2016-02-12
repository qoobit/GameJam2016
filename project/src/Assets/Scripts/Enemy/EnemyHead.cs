using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHead : MonoBehaviour
{
    public enum State { IDLE, SEARCHING, LOCKED, LOST }

    // Public parameters
    public State state = State.IDLE;
    public float rotationLimitY = 75.0f; // How far we can turn our neck
    public float rotationLimitX = 10.0f;
    public float rotationMaxVelocity = 135.0f; // How fast we can turn our head
    public float sightDistance = 15.0f; //How far we can see
    public float sightRadius = 0.5f; // How narrow our line of sight is
    public float targetLostTimeLimit = 4.0f; // If target is out of sight, how long before we give up  
    public Transform lookAtTarget; //The target to lock on to

    // Private variables for calculations
    private Vector3 targetForward;
    private float targetLastSeenTime; //Keeps track of when we last saw the target
    public List<GameObject> searchTargets; //List of targets to search for

    // Private references
    private Transform head;
    private Transform body;

    // Use this for initialization
    void Start()
    {
        head = this.transform.FindChild("Head");
        body = this.transform.FindChild("Body");

        if (head == null) throw new System.Exception("Unable to find Head");
        if (body == null) throw new System.Exception("Unable to find Body");

        targetForward = Vector3.forward;
    }

    void Update()
    {
        this.updateHeadRotation();

        switch (state)
        {
            case State.IDLE:
                this.updateIdle();
                break;

            case State.SEARCHING:
                this.updateSearching();
                break;

            case State.LOCKED:
                this.updateLocked();
                break;

            case State.LOST:
                this.updateLost();
                break;
        }
    }

    private void updateHeadRotation() //todo: fix our target forward so it's relative to our body (fix the debug too)
    {
        head.forward = Vector3.RotateTowards(head.forward, body.transform.rotation * targetForward, Mathf.Deg2Rad * rotationMaxVelocity * Time.deltaTime, 0.0f);
        Debug.DrawRay(head.position, body.transform.rotation * targetForward * sightDistance, Color.cyan);
        Debug.DrawRay(head.position, head.forward * sightDistance, Color.red);
    }

    private void updateIdle()
    {
        targetForward = Vector3.forward;
    }

    private void updateSearching()
    {
        //If we are close enough to our targetRotation, get next targetRotation
        float remainder = Vector3.Angle(head.forward, body.transform.rotation * targetForward);
        if (remainder <= 0.5f)
            targetForward = this.getNextSearchTargetForward();

        this.searchForTarget();
    }

    private void searchForTarget()
    {
        if (searchTargets == null)
            return;

        RaycastHit hitInfo;
        Physics.SphereCast(head.position, sightRadius, head.forward, out hitInfo, sightDistance - sightRadius);

        if (hitInfo.collider == null)
            return;

        //Check if the collider belongs to our target
        Transform currentTransform = hitInfo.collider.transform;
        do
        {
            foreach (GameObject searchTarget in searchTargets)
            {
                if (currentTransform == searchTarget.transform)
                {
                    state = State.LOCKED;
                    lookAtTarget = searchTarget.transform;
                    targetLastSeenTime = Time.time;
                    return;
                }
            }

            currentTransform = currentTransform.parent;
        }
        while (currentTransform != null);
    }

    private void updateLocked()
    {
        this.lockOnTarget();
    }

    private void updateLost()
    {
        this.lockOnTarget();
    }

    public Vector3 rotationToTargetEuler;

    private void lockOnTarget()
    {
        //If we haven't seen the target in a while, change our state to SEARCHING
        if (lookAtTarget == null || Time.time > (this.targetLastSeenTime + this.targetLostTimeLimit))
        {
            state = State.SEARCHING;
            return;
        }
        
        Vector3 vectorToTarget = lookAtTarget.position - head.position;
        Vector3 rotationToTarget = Quaternion.FromToRotation(body.transform.forward, vectorToTarget.normalized).eulerAngles;


        //If we can see the target, look at the target
        // and check if this rotation is within our limits
        if (vectorToTarget.magnitude <= this.sightDistance &&
            Mathf.Abs(Common.Calculations.ClampAngle180(rotationToTarget.x)) <= rotationLimitX &&
            Mathf.Abs(Common.Calculations.ClampAngle180(rotationToTarget.y)) <= rotationLimitY)
        {
            state = State.LOCKED;
            targetLastSeenTime = Time.time;
            targetForward = (Quaternion.Inverse(body.transform.rotation) * vectorToTarget).normalized;

            /*
            Vector3 vectorToTargetProjected = Vector3.ProjectOnPlane(vectorToTarget, body.up).normalized;

            float angleYToTarget = Vector3.Angle(body.transform.forward, vectorToTargetProjected);
            if (angleYToTarget <= this.rotationLimitY)
                targetForward = Quaternion.Inverse(body.transform.rotation) * vectorToTargetProjected;
            */
        }
        else
        {
            state = State.LOST;
        }
    }

    private Vector3 lookLeftVector3()
    {
        float rad = Mathf.Deg2Rad * rotationLimitY;
        float x = -Mathf.Sin(rad);
        float y = 0f;
        float z = Mathf.Cos(rad);
        return new Vector3(x, y, z).normalized;
    }

    private Vector3 lookRightVector3()
    {
        float rad = Mathf.Deg2Rad * rotationLimitY;
        float x = Mathf.Sin(rad);
        float y = 0f;
        float z = Mathf.Cos(rad);
        return new Vector3(x, y, z).normalized;
    }

    private Vector3 getRandomSearchTargetForward()
    {
        if (Random.Range(0, 1) == 0)
            return lookLeftVector3();
        else
            return lookRightVector3();
    }

    private Vector3 getNextSearchTargetForward()
    {
        Vector3 left = lookLeftVector3();
        if (targetForward.Equals(left))
            return lookRightVector3();
        else
            return left;
    }

    public void SearchForTargets(List<GameObject> searchTargets)
    {
        this.searchTargets = searchTargets;
        if (state != State.SEARCHING)
        {
            targetForward = this.getRandomSearchTargetForward();
            state = State.SEARCHING;
        }
    }

    public void LookAt(Transform target)
    {
        lookAtTarget = target;
        state = State.LOCKED;
    }
}
