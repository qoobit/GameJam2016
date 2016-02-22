using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : Enemy
{
    public enum State { WANDER, ATTACK, PROJECTILE }

    [Header("Turret Parameters")]

    // Public parameters
    public float attackAngleLimit = 10.0f; //Fire our weapon if we are facing our target within n degrees

    // Private references
    private EnemyHead head;
    private Transform body;
    private EnemyWalk enemyWalk;
    private Animator animator;
    private Rigidbody rigidBody;

    //For Animation States
    private bool isAttacking;

    public Vector3 Velocity;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        head = GetComponent<EnemyHead>();
        body = this.transform.FindChild("Body");
        rigidBody = GetComponent<Rigidbody>();
        enemyWalk = GetComponent<EnemyWalk>();
        animator = GetComponent<Animator>();

        //Load a blaster as our weapon
        GameObject blasterWeapon = GameControl.Spawn(Spawnable.Type.WEAPON_BLASTER, gameObject.transform.position, gameObject.transform.rotation);
        blasterWeapon.transform.parent = body;
        weapon = blasterWeapon.GetComponent<Weapon>();
        weapon.weaponFireMode = 0;

        //Set our head to search
        List<GameObject> heroList = GameControl.control.level.GetHeroList();
        head.SearchForTargets(heroList);
    }

    // Update is called once per frame
    protected override void Update()
    {
        Velocity = rigidBody.velocity;

        base.Update();

        switch (currentState)
        {
            case (int)State.WANDER:
                updateWander();
                break;

            case (int)State.PROJECTILE:
                updateProjectile();
                break;

            case (int)State.ATTACK:
                updateAttack();
                break;

        }
        updateAnimationStates();
    }

    private void updateWander()
    {
        isAttacking = false;
        enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;

        if (head.state == EnemyHead.State.LOCKED)
        {
            setCurrentState((int)State.ATTACK);
        }
    }

    private void updateProjectile()
    {
        isAttacking = false;
        enemyWalk.state = EnemyWalk.State.DISABLE_NAVMESH;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.transform.position, out hit, 0.5f, 1))
        {
            rigidBody.velocity = Vector3.zero;
            setCurrentState((int)State.WANDER);
        }
    }

    private void updateAttack()
    {
        if (head.state == EnemyHead.State.SEARCHING)
        {
            setCurrentState((int)State.WANDER);
        }
        else
        {
            isAttacking = true;
            enemyWalk.lookAtTarget = head.lookAtTarget;
            enemyWalk.state = EnemyWalk.State.FOLLOW_TARGET;
            fireWeapon();
        }
    }

    private void fireWeapon()
    {
        if (weapon == null) return;

        //If we are looking in the general direction of our target, fire our weapon
        if (Vector3.Angle(body.transform.forward, head.lookAtTarget.position - body.transform.position) <= attackAngleLimit)
        {
            weapon.Fire();
        }
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
