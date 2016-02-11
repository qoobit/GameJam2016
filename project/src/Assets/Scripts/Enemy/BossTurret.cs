using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossTurret : Enemy
{
    //Private references
    private EnemyHead head;
    private Transform body;
    private EnemyWalk enemyWalk;
    private Animator animator;
    private Rigidbody rigidBody;

    //For Animation States
    private bool isAttacking;

    //Dash Attack variables
    private float dashSpeed = 50.0f;
    private float dashChargeDuration = 1.25f;
    private float dashAttackDuration = 0.75f;
    private float dashFinishedDuration = 1.0f;
    private Vector3 dashTargetPosition;
    private float dashTargetRemaining;

    //Jump Attack variables
    private float jumpSpeed = 50f;

    //Fire Weapon variables
    private float fireWeaponDuration = 2.0f;

    private enum BossTurretState
    {
        WANDER,                                         //Wander around
        DASH_CHARGING, DASH_ATTACKING, DASH_FINISHED,   //Dash attack
        JUMP_START, JUMP_RISING, FALLING,               //Jump attack
        FIRE_WEAPON                                     //Fire weapon
    };

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        head = GetComponent<EnemyHead>();
        body = this.transform.FindChild("Body");
        enemyWalk = GetComponent<EnemyWalk>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();

        //Load a blaster as our weapon
        GameObject turretCannon = GameControl.Spawn(Spawnable.Type.WEAPON_TURRET_CANNON, gameObject.transform.position, gameObject.transform.rotation);
        turretCannon.transform.parent = body;
        weapon = turretCannon.GetComponent<Weapon>();
        turretCannon.GetComponent<TurretCannon>().waypointCollection = enemyWalk.waypointCollection;

        //Set our initial state to wander
        setCurrentState((int)BossTurretState.WANDER);
        head.state = EnemyHead.State.IDLE;
        enemyWalk.state = EnemyWalk.State.IDLE;

        //Set our head to search
        List<GameObject> heroList = GameControl.control.level.GetHeroList();
        head.SearchForTargets(heroList);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case (int)BossTurretState.WANDER:
                updateWander();
                break;
            case (int)BossTurretState.DASH_CHARGING:
                updateDashCharge();
                break;
            case (int)BossTurretState.DASH_ATTACKING:
                updateDashAttack();
                break;
            case (int)BossTurretState.DASH_FINISHED:
                updateDashFinish();
                break;
            case (int)BossTurretState.JUMP_START:
                updateJumpStart();
                break;
            case (int)BossTurretState.JUMP_RISING:
                updateJumpRising();
                break;
            case (int)BossTurretState.FALLING:
                updateJumpFall();
                break;
            case (int)BossTurretState.FIRE_WEAPON:
                updateFireWeapon();
                break;
        }

        updateAnimationStates();
    }

    private void updateWander()
    {
        if (head.state == EnemyHead.State.LOCKED)
        {
            attack();
        }
        else
        {
            head.state = EnemyHead.State.SEARCHING;
            enemyWalk.state = EnemyWalk.State.RANDOM;
            isAttacking = false;
        }
    }

    private void attack()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                setCurrentState((int)BossTurretState.DASH_CHARGING);
                updateDashCharge();
                break;
            case 1:
                setCurrentState((int)BossTurretState.JUMP_START);
                updateJumpStart();
                break;
            case 2:
                setCurrentState((int)BossTurretState.FIRE_WEAPON);
                updateFireWeapon();
                break;
        }
    }

    private void updateDashCharge()
    {
        setNextState((int)BossTurretState.DASH_ATTACKING, dashChargeDuration);

        enemyWalk.state = EnemyWalk.State.IDLE;
        isAttacking = true; //Just for animation
        dashTargetRemaining = float.MaxValue;

        //Turn body towards target
        this.transform.LookAt(head.lookAtTarget);
        rigidBody.velocity = Vector3.zero;
        dashTargetPosition = head.lookAtTarget.position;
    }

    private void updateDashAttack()
    {
        //Check if we've reached our destination yet
        float currentDashTargetRemaining = (dashTargetPosition - this.transform.position).magnitude;
        if (currentDashTargetRemaining <= dashTargetRemaining)
        {
            Vector3 forwardNoY = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z);
            rigidBody.velocity = forwardNoY.normalized * dashSpeed;
            dashTargetRemaining = currentDashTargetRemaining;
            enemyWalk.state = EnemyWalk.State.IDLE;
            head.state = EnemyHead.State.IDLE;
            isAttacking = false; //Just for animation

            setNextState((int)BossTurretState.DASH_FINISHED, dashAttackDuration);
        }
        else
        {
            //We reached our destination
            setCurrentState((int)BossTurretState.DASH_FINISHED);
            updateDashFinish();
        }
    }

    private void updateDashFinish()
    {
        //stand idle to recover from dash
        enemyWalk.state = EnemyWalk.State.IDLE;
        head.state = EnemyHead.State.IDLE;
        rigidBody.velocity = Vector3.zero;
        isAttacking = false;

        setNextState((int)BossTurretState.WANDER, dashFinishedDuration);
    }

    private void updateJumpStart()
    {
        enemyWalk.state = EnemyWalk.State.IDLE;

        Vector3 jumpVelocity;
        if (Common.Calculations.ProjectileLaunchVector3(this.transform.position, head.lookAtTarget.position, jumpSpeed, out jumpVelocity, 0f))
        {
            rigidBody.velocity = jumpVelocity;
        }

        setCurrentState((int)BossTurretState.JUMP_RISING);
    }

    private void updateJumpRising()
    {
        enemyWalk.state = EnemyWalk.State.DISABLE_NAVMESH;
        head.state = EnemyHead.State.IDLE;

        float angleFromGravity = Vector3.Angle(Physics.gravity, rigidBody.velocity);
        if (angleFromGravity <= 90f)
        {
            setCurrentState((int)BossTurretState.FALLING);
            updateJumpFall();
        }
    }

    private void updateJumpFall()
    {
        enemyWalk.state = EnemyWalk.State.IDLE;
        head.state = EnemyHead.State.IDLE;

        setCurrentState((int)BossTurretState.WANDER);
    }

    private void updateFireWeapon()
    {
        setNextState((int)BossTurretState.WANDER, fireWeaponDuration);

        enemyWalk.state = EnemyWalk.State.IDLE;
        isAttacking = true; //Just for animation

        //Turn body towards target
        this.transform.LookAt(head.lookAtTarget);
        rigidBody.velocity = Vector3.zero;

        if (weapon != null)
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
