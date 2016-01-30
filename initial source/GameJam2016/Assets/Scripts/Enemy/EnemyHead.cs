using UnityEngine;
using System.Collections;

public enum EnemyHeadState { SEARCHING, LOCKED }

public class EnemyHead : MonoBehaviour
{
    public float RotationPeriod = 2.0f;
    public float RotationLimitX = 75.0f;
    public float RotationLimitY = 10.0f;

    private Transform head;
    private Transform body;
    private EnemyHeadState currentState;
    private Transform lookAtTarget;
    private float searchStartTime = 0.0f;
    private bool searchingLeft = true;

    // Use this for initialization
    void Start()
    {
        head = this.transform.FindChild("Head");
        body = this.transform.FindChild("Body");
        if (head == null)
            throw new System.Exception("Unable to find Head");
        if (body == null)
            throw new System.Exception("Unable to find Body");
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case EnemyHeadState.SEARCHING:
                this.updateSearching();
                break;

            case EnemyHeadState.LOCKED:
                this.updateLocked();
                break;
        }
    }

    private void updateSearching()
    {
        if (this.searchStartTime <= 0.0f) //Initialize the value if not set
            this.searchStartTime = Time.time;

        float deltaTime = Time.time - this.searchStartTime;

        Quaternion lookingLeft = Quaternion.AngleAxis(-RotationLimitX, Vector3.up); //Looking left
        Quaternion lookingRight = Quaternion.AngleAxis(RotationLimitX, Vector3.up); //Looking right

        float halfPeriod = this.RotationPeriod / 2;

        float remaining = 0f;
        if (this.searchingLeft == true)
        {
            head.localRotation = Quaternion.Lerp(lookingRight, lookingLeft, (deltaTime % halfPeriod) / halfPeriod);
            remaining = Quaternion.Angle(head.localRotation, lookingLeft);
        }
        else
        {
            head.localRotation = Quaternion.Lerp(lookingLeft, lookingRight, (deltaTime % halfPeriod) / halfPeriod);
            remaining = Quaternion.Angle(head.localRotation, lookingRight);
        }

        if (remaining <= 5.0f)
        {
            this.searchStartTime = Time.time;
            this.searchingLeft = !this.searchingLeft;
        }

        Debug.DrawRay(head.position, head.forward * 200, Color.red);
    }

    private void updateLocked()
    {
        if (lookAtTarget == null)
            return;

        head.LookAt(lookAtTarget);
    }

    public void LookForPlayer()
    {
        if (currentState != EnemyHeadState.SEARCHING)
        {
            this.searchStartTime = Time.time;
            currentState = EnemyHeadState.SEARCHING;
        }
    }

    public void LookAt(Transform target)
    {
        lookAtTarget = target;
        currentState = EnemyHeadState.LOCKED;
    }
}
