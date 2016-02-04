using UnityEngine;
using System.Collections;

public class BossTurret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyBase enemyBase;
    private Animator animator;

    private NavMeshAgent navMeshAgent;
    private Rigidbody rigidBody;

    //For Animation States
    private bool isAttacking;

    //Attack variables
    private float dashAttackChargeStart = -1f;
    private float dashAttackChargingStart = -1f;
    private float dashAttackChargeDuration = 2.0f;
    private float dashAttackChargingDuration = 0.75f;
    private float dashAttackSpeed = 50.0f;
    private Vector3 dashTargetPosition;
    private float dashTargetRemaining;
    private bool dashTargetLocked;



    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        head = GetComponent<EnemyHead>();
        body = this.transform.FindChild("Body");
        enemyBase = GetComponent<EnemyBase>();
        animator = GetComponent<Animator>();

        if (head == null) throw new System.Exception("Unable to find Head");
        if (enemyBase == null) throw new System.Exception("Unable to find Body");

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
        base.Update();
        if (enemyBase.CurrentState == EnemyBaseState.OFFENSE)
        {
            if (head.currentState == EnemyHeadState.LOCKED)
            {
                int rand = Random.Range(0, 1);
                switch (rand)
                {
                    case 0:
                        dashAttack();
                        break;
                    case 1:
                        dashAttack();
                        //jumpAttack();
                        break;
                }
            }
            else
            {
                isAttacking = false;
            }
        }

        updateAnimationStates();
    }

    private void dashAttack()
    {
        if (dashAttackChargeStart < 0)
        {
            dashAttackChargeStart = Time.time;
        }
        else if (Time.time >= dashAttackChargeStart + dashAttackChargeDuration)
        {
            if (dashAttackChargingStart < 0)
            {
                dashAttackChargingStart = Time.time;
                dashTargetLocked = false;
            }
            else if (Time.time < dashAttackChargingStart + dashAttackChargingDuration)
            {
                float currentDashTargetRemaining;

                //Do dash attack
                if (!dashTargetLocked)
                {
                    dashTargetPosition = head.lookAtTarget.position;
                    dashTargetRemaining = float.MaxValue;
                    dashTargetLocked = true;
                }

                currentDashTargetRemaining = (dashTargetPosition - this.transform.position).magnitude;
                if (currentDashTargetRemaining > dashTargetRemaining)
                    dashAttackFinish();
                else
                {
                    navMeshAgent.enabled = false;
                    Vector3 forwardNoY = new Vector3(this.transform.forward.x, 0f, this.transform.forward.z);
                    rigidBody.velocity = forwardNoY.normalized * dashAttackSpeed;
                    isAttacking = false; //Just for animation
                    dashTargetRemaining = currentDashTargetRemaining;
                }
            }
            else
            {
                dashAttackFinish();
            }
        }
        else
        {
            //Do Charging up
            isAttacking = true; //Just for animation
            rigidBody.velocity = Vector3.zero;
        }
    }

    private void dashAttackFinish()
    {
        enemyBase.CurrentState = EnemyBaseState.PATROL;
        rigidBody.velocity = Vector3.zero;
        navMeshAgent.enabled = true;
        isAttacking = false;
        dashAttackChargeStart = -1f;
        dashAttackChargingStart = -1f;
    }

    private void jumpAttack()
    {
        
    }

    private void jumpAttackFinish()
    {
        enemyBase.CurrentState = EnemyBaseState.PATROL;
        rigidBody.velocity = Vector3.zero;
        navMeshAgent.enabled = true;
        isAttacking = false;
    }

    private void fireWeapon()
    {
        if (weapon == null) return;

        weapon.Fire();
    }

    private void updateAnimationStates()
    {
        animator.SetBool("attacking", isAttacking);
        animator.SetFloat("speed", enemyBase.GetVelocity().magnitude);
        animator.SetFloat("hp", this.hp);
    }

    override public void Die()
    {
        baseDamage = 0f;
        StartCoroutine(WaitAndExplode(2.5f));
    }
}
