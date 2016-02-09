using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyWalk enemyWalk;
    private Animator animator;

    //For Animation States
    private bool isAttacking;

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

        //Set our head to search
        List<GameObject> heroList = GameControl.control.level.GetHeroList();
        head.SearchForTargets(heroList);

        //Set our inital walk state
        enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

        isAttacking = false;

        if (head.state == EnemyHead.State.LOCKED)
        {
            enemyWalk.lookAtTarget = head.lookAtTarget;
            enemyWalk.state = EnemyWalk.State.FOLLOW_TARGET;
            isAttacking = true;
            attack();
        }
        else if (head.state == EnemyHead.State.SEARCHING)
        {
            enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;
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
        animator.SetFloat("speed", enemyWalk.GetVelocity().magnitude);
        animator.SetFloat("hp", this.hp);
    }

    override public void Die()
    {
        baseDamage = 0f;
        StartCoroutine(WaitAndExplode(2.5f));
    }
}
