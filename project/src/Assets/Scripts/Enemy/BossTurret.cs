using UnityEngine;
using System.Collections;

public class BossTurret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyBase enemyBase;
    private Animator animator;

    private NavMeshAgent bossAgent;

    //For Animation States
    private bool isAttacking;

    //Attack variables
    float dashAttackChargeStart = -1f;
    float dashAttackChargingStart = -1f;
    float dashAttackChargeDuration = 2.0f;
    float dashAttackChargingDuration = 0.75f;
    float dashAttackSpeed = 50.0f;



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

        bossAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (enemyBase.CurrentState == EnemyBaseState.OFFENSE)
        {
            if (head.currentState == EnemyHeadState.LOCKED)
            {
                dashAttack();
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
            }
            else if (Time.time < dashAttackChargingStart + dashAttackChargingDuration)
            {
                //Do charging attack
                bossAgent.enabled = false;
                this.transform.position += this.transform.forward * dashAttackSpeed * Time.deltaTime;
                isAttacking = false;
            }
            else
            {
                //Finished Charging Attack
                enemyBase.CurrentState = EnemyBaseState.PATROL;
                bossAgent.enabled = true;
                isAttacking = false;
                dashAttackChargeStart = -1f;
                dashAttackChargingStart = -1f;
            }
        }
        else
        {
            //Do Charging up
            isAttacking = true;
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
        animator.SetFloat("speed", enemyBase.GetVelocity().magnitude);
        animator.SetFloat("hp", this.hp);
    }

    override public void Die()
    {
        baseDamage = 0f;
        StartCoroutine(WaitAndExplode(2.5f));
    }
}
