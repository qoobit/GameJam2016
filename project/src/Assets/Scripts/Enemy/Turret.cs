using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : Enemy
{

    [Header("Turret Parameters")]


    // Public parameters
    public float attackAngleLimit = 10.0f; //Fire our weapon if we are facing our target within n degrees
    public float frictionCoefficient = 0.2f; //Apply friction when Turret is shot as a projectile from TurretCannon

    // Private references
    private EnemyHead head;
    private Transform body;
    private EnemyWalk enemyWalk;
    private Animator animator;
    private Rigidbody rigidBody;

    //For Animation States
    private bool isAttacking;

    public Vector3 velocity;

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

        //Set our inital walk state
        enemyWalk.state = EnemyWalk.State.WAYPOINT_RANDOM;
    }

    // Update is called once per frame
    protected override void Update()
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

        applyFriction();

        updateAnimationStates();
    }

    private void attack()
    {
        if (weapon == null) return;

        //If we are looking in the general direction of our target, fire our weapon
        if (Vector3.Angle(body.transform.forward, head.lookAtTarget.position - body.transform.position) <= attackAngleLimit)
        {
            weapon.Fire();
        }
    }

    private void applyFriction()
    {
        rigidBody.velocity *= (1 - (frictionCoefficient * Time.deltaTime));
        if (rigidBody.velocity.magnitude <= 0.5f)
            rigidBody.velocity = Vector3.zero;
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
