using UnityEngine;
using System.Collections;

public enum EnemyHeadState { IDLE, SEARCHING, LOCKED, LOST }

public class EnemyHead : MonoBehaviour
{
    public float RotationLimitY = 75.0f;
    public float RotationLimitX = 10.0f;
    public float RotationMaxVelocity = 135.0f;
    public float SphereCastRadius = 0.5f;
    public float SightDistance = 15.0f;
    public float TargetLostTimeout = 4.0f; // If target is out of sight, how long before we give up



    private Transform head;
    private Transform body;
    public EnemyHeadState currentState;
    public Transform lookAtTarget;
    private Quaternion targetRotation;
    private Quaternion lookingLeft;
    private Quaternion lookingRight;
    private float targetLastSeenTime; //Keeps track of when we last saw the target

    // Use this for initialization
    void Start()
    {
        head = this.transform.FindChild("Head");
        body = this.transform.FindChild("Body");

        if (head == null) throw new System.Exception("Unable to find Head");
        if (body == null) throw new System.Exception("Unable to find Body");

        targetRotation = head.localRotation;
        lookingLeft = Quaternion.AngleAxis(-RotationLimitY, Vector3.up);
        lookingRight = Quaternion.AngleAxis(RotationLimitY, Vector3.up);
}

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case EnemyHeadState.IDLE:
                break;

            case EnemyHeadState.SEARCHING:
                this.updateSearching();
                this.lookForHero();
                break;

            case EnemyHeadState.LOCKED:
            case EnemyHeadState.LOST:
                this.updateLockedAndLost();
                break;
        }
    }
    
    private void updateSearching()
    {
        head.localRotation = Quaternion.RotateTowards(head.localRotation, targetRotation, this.RotationMaxVelocity * Time.deltaTime);
        
        //Project head.forward vector on to body.up plane
        Vector3 headForwardProjected = this.projectVector3OnPlane(head.forward, body.up).normalized;

        //Get our current Y rotation
        float headRotationY = Vector3.Angle(headForwardProjected, body.forward);

        //Check if we have over rotated
        if (headRotationY > this.RotationLimitY)
            head.localRotation = targetRotation;

        //If we are close enough to our targetRotation, get next targetRotation
        float remainder = Quaternion.Angle(head.localRotation, targetRotation);
        if (remainder <= 0.5f)
            targetRotation = this.getNextSearchTargetRotation();

        Debug.DrawRay(head.position, head.forward * SightDistance, Color.red);
        return;
    }

    private Vector3 projectVector3OnPlane(Vector3 v, Vector3 planeNormal)
    {
        float dot = -Vector3.Dot(v, planeNormal);
        return (v + (planeNormal * dot));
    }

    private void lookForHero()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(head.position, SphereCastRadius, head.forward, out hitInfo, SightDistance);

        if (hitInfo.collider == null)
            return;

        //Check if the collider belongs to Hero
        Transform currentTransform = hitInfo.collider.transform;
        do
        {
            if (currentTransform.name == "Hero")
            {
                this.LookAt(currentTransform);
                targetLastSeenTime = Time.time;
                currentState = EnemyHeadState.LOCKED;
                break;
            }
            else
            {
                currentTransform = currentTransform.parent;
            }
        } while (currentTransform != null);
    }

    private void updateLockedAndLost()
    {
        if (lookAtTarget == null)
            currentState = EnemyHeadState.IDLE;

        //If we haven't seen the target in a while, change our state
        if ((Time.time - targetLastSeenTime) >= this.TargetLostTimeout)
        {
            currentState = EnemyHeadState.IDLE;
            return;
        }

        //If we can see the target, look at the target
        Vector3 vectorToTarget = lookAtTarget.position - head.position;
        if (vectorToTarget.magnitude <= this.SightDistance)
        {
            currentState = EnemyHeadState.LOCKED;
            Vector3 vectorToTargetProjected = this.projectVector3OnPlane(vectorToTarget, body.up).normalized;

            float angleYToTarget = Vector3.Angle(body.transform.forward, vectorToTargetProjected);
            if (angleYToTarget <= this.RotationLimitY)
                head.LookAt(lookAtTarget);

            targetLastSeenTime = Time.time;
        }
        else
        {
            currentState = EnemyHeadState.LOST;
        }
    }

    public void LookForPlayer()
    {
        if (currentState != EnemyHeadState.SEARCHING)
        {
            targetRotation = this.getRandomSearchTargetRotation();
            currentState = EnemyHeadState.SEARCHING;
        }
    }

    public void LookAt(Transform target)
    {
        lookAtTarget = target;
        currentState = EnemyHeadState.LOCKED;
    }



    private Quaternion getRandomSearchTargetRotation()
    {
        if (Random.Range(0, 1) == 0)
            return lookingLeft;
        else
            return lookingRight;
    }

    private Quaternion getNextSearchTargetRotation()
    {
        if (targetRotation.Equals(lookingLeft))
            return lookingRight;
        else
            return lookingLeft;
    }
}
