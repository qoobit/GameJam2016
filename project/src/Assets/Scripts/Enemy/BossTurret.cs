using UnityEngine;
using System.Collections;

public class BossTurret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyBase enemyBase;
    private Animator animator;

    //For Animation States
    private bool isAttacking;

    //Attack variables
    float dashAttackChargeStart = -1f;
    float dashAttackChargingStart = -1f;
    float dashAttackChargeDuration = 2.0f;
    float dashAttackChargingDuration = 0.75f;



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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (enemyBase.CurrentState == EnemyBaseState.OFFENSE)
        {
            if (head.currentState == EnemyHeadState.LOCKED)
            {
                isAttacking = true;
                fireWeapon();
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
        else if (Time.time < dashAttackChargeStart + dashAttackChargeDuration)
        {
            if (dashAttackChargingStart < 0)
            {
                dashAttackChargingStart = Time.time;
            }
            else if (Time.time < dashAttackChargingStart + dashAttackChargingDuration)
            {

            }
            else
            {
                //Do charging attack
            }
        }
        else
        {
            //Do Charging up
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
