using UnityEngine;
using System.Collections;

public enum EnemyHeadState { SEARCHING, LOCKED }

public class EnemyHead : MonoBehaviour
{
    public float RotationLimitY = 75.0f;
    public float RotationLimitX = 10.0f;
    public float RotationMaxVelocity = 135.0f;
    public float SphereCastRadius = 0.5f;
    public float SphereCastDistance = 15.0f;

    private Transform head;
    private Transform body;
    public EnemyHeadState currentState;
    public Transform lookAtTarget;
    private Quaternion targetRotation;
    private Quaternion lookingLeft;
    private Quaternion lookingRight;

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
            case EnemyHeadState.SEARCHING:
                this.updateSearching();
                this.lookForHero();
                break;

            case EnemyHeadState.LOCKED:
                this.updateLocked();
                break;
        }
    }
    
    private void updateSearching()
    {
        //Quaternion localHeadRotation = 
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

        Debug.DrawRay(head.position, head.forward * SphereCastDistance, Color.red);
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
        Physics.SphereCast(head.position, SphereCastRadius, head.forward, out hitInfo, SphereCastDistance);

        if (hitInfo.collider == null)
            return;

        //Check if the collider belongs to Hero
        Transform currentTransform = hitInfo.collider.transform;
        do
        {
            if (currentTransform.name == "Hero")
            {
                this.LookAt(currentTransform);
                currentState = EnemyHeadState.LOCKED;
                break;
            }
            else
            {
                currentTransform = currentTransform.parent;
            }
        } while (currentTransform != null);
    }

    private void updateLocked()
    {
        if (lookAtTarget == null)
            return;

        Vector3 directionToTarget = lookAtTarget.position - head.position;
        Vector3 directionToTargetProjected = this.projectVector3OnPlane(directionToTarget, body.up).normalized;

        float angleYToTarget = Vector3.Angle(body.transform.forward, directionToTargetProjected);
        if (angleYToTarget <= this.RotationLimitY)
            head.LookAt(lookAtTarget);
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
