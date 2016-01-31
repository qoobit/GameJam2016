using UnityEngine;
using System.Collections;

public class Turret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyBase enemyBase;
    private Animator animator;

    //For Animation States
    private bool isAttacking;

    // Use this for initialization
    void Start()
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
    void Update ()
    {
        base.Update();
        if (enemyBase.CurrentState == EnemyBaseState.OFFENSE)
        {
            if (head.currentState == EnemyHeadState.LOCKED)
            {
                isAttacking = true;
                attack();
            }
            else
            {
                isAttacking = false;
            }
        }

        updateAnimationStates();
    }

    private void attack()
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
