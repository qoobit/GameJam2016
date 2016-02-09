using UnityEngine;
using System.Collections;

public class BossTurret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyWalk enemyWalk;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidBody;

    //For Animation States
    private bool isAttacking;

    //Dash Attack variables
    private float dashSpeed = 50.0f;
    private float dashChargeDuration = 2.0f;
    private float dashAttackDuration = 0.75f;
    private Vector3 dashTargetPosition;
    private float dashTargetRemaining;

    //Jump Attack variables
    private float jumpSpeed = 50f;

    private enum BossTurretState {IDLE, DASH_CHARGING, DASH_ATTACKING, DASH_FINISHED, JUMP_START, JUMP_RISING, FALLING};
    public bool navMesh = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        head = GetComponent<EnemyHead>();
        body = this.transform.FindChild("Body");
        enemyWalk = GetComponent<EnemyWalk>();
        animator = GetComponent<Animator>();

        if (head == null) throw new System.Exception("Unable to find Head");
        if (enemyWalk == null) throw new System.Exception("Unable to find Enemy Walk");

        //Load a blaster as our weapon
        Object blasterObject = Resources.Load("Weapons/Blaster", typeof(GameObject));
        GameObject blasterWeapon = GameObject.Instantiate(blasterObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        blasterWeapon.transform.parent = body;
        weapon = blasterWeapon.GetComponent<Weapon>();
        weapon.weaponFireMode = 0;

        navMeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        navMesh = navMeshAgent.isActiveAndEnabled;
        base.Update();
        if (Input.GetKeyDown(KeyCode.J))
            setCurrentState((int)BossTurretState.JUMP_START);

        switch(currentState)
        {
            case (int)BossTurretState.IDLE:
                idle();
                break;
            case (int)BossTurretState.DASH_CHARGING:
                dashCharge();
                break;
            case (int)BossTurretState.DASH_ATTACKING:
                dashAttack();
                break;
            case (int)BossTurretState.DASH_FINISHED:
                dashFinish();
                break;
            case (int)BossTurretState.JUMP_START:
                jumpStart();
                break;
            case (int)BossTurretState.JUMP_RISING:
                jumpRising();
                break;
            case (int)BossTurretState.FALLING:
                jumpFall();
                break;
        }

        updateAnimationStates();
    }

    private void idle()
    {
        /*
        if (enemyWalk.state == EnemyWalk.State.OFFENSE)
        {
            if (head.currentState == EnemyHeadState.LOCKED)
            {
                int rand = Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        //setNextState((int)BossTurretState.DASH_CHARGING, 0);
                        setNextState((int)BossTurretState.JUMP_START, 0);
                        break;
                    case 1:
                        setNextState((int)BossTurretState.JUMP_START, 0);
                        break;
                }
            }
            else
            {
                isAttacking = false;
            }
        }*/
    }

    private void dashCharge()
    {
        //Do Charging up
        isAttacking = true; //Just for animation
        navMeshAgent.Stop(); // Stop moving
        rigidBody.velocity = Vector3.zero;
        dashTargetPosition = head.lookAtTarget.position;
        dashTargetRemaining = float.MaxValue;

        setNextState((int)BossTurretState.DASH_ATTACKING, dashChargeDuration);
    }

    private void dashAttack()
    {
        //Do dash attack
        float currentDashTargetRemaining = (dashTargetPosition - this.transform.position).magnitude;
        if (currentDashTargetRemaining > dashTargetRemaining)
        {
            dashFinish(); 
        }
        else
        {
            navMeshAgent.Stop();
            Vector3 forwardNoY = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z);
            rigidBody.velocity = forwardNoY.normalized * dashSpeed;
            isAttacking = false; //Just for animation
            dashTargetRemaining = currentDashTargetRemaining;

            setNextState((int)BossTurretState.DASH_FINISHED, dashAttackDuration);
        }
    }

    private void dashFinish()
    {
        enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;
        rigidBody.velocity = Vector3.zero;
        navMeshAgent.Resume();
        isAttacking = false;

        setCurrentState((int)BossTurretState.IDLE);
    }

    private void jumpStart()
    {
        //Disable the nav mesh agent
        Debug.Log("Jump starting... ");
        navMeshAgent.enabled = false;
        
        Vector3 jumpVelocity;
        //if (Common.Calculations.ProjectileLaunchVector3(this.transform.position, head.lookAtTarget.position, jumpSpeed, out jumpVelocity, 0f))
        if (Common.Calculations.ProjectileLaunchVector3(this.transform.position, Vector3.zero, jumpSpeed, out jumpVelocity, 0f))
        {
            //navMeshAgent.enabled = false;
            rigidBody.velocity = jumpVelocity;
            Debug.Log("jump velocity: " + jumpVelocity.ToString());
        }

        setCurrentState((int)BossTurretState.JUMP_RISING);
    }

    private void jumpRising()
    {
        float angleFromGravity = Vector3.Angle(Physics.gravity, rigidBody.velocity);
        Debug.Log("Jump rising. Angle From Gravity: " + angleFromGravity);
        if (angleFromGravity <= 90f)
            setCurrentState((int)BossTurretState.FALLING);
    }

    private void jumpFall()
    {
        Debug.Log("Jump falling ");
        
        
        float maxDistance = 2.0f;
        float threshold = 2.0f;
        NavMeshHit hit;

        //Check how close we are to navmesh
        if (NavMesh.SamplePosition(this.transform.position, out hit, maxDistance, 1) == false)
            return;

        //Re-enable the navmesh when we are close enough
        if (hit.distance <= threshold)
        { 
            navMeshAgent.enabled = true;
            enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;
            rigidBody.velocity = Vector3.zero;
            isAttacking = false;

            setCurrentState((int)BossTurretState.IDLE);
        }
    }

    private void fireWeapon()
    {
        if (weapon == null) return;

        weapon.Fire();
    }

    private void updateAnimationStates()
    {
        animator.SetBool("attacking", isAttacking);
        animator.SetFloat("speed", enemyWalk.GetVelocity().magnitude);
        animator.SetFloat("hp", this.hp);
    }

    override public void Die()
    {
        base.Die();
    }
}
