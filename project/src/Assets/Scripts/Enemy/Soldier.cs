using UnityEngine;
using System.Collections;

public class Soldier : Enemy
{
    private EnemyHead head;
    private Transform body;
    private EnemyBase enemyBase;

    // Use this for initialization
    void Start()
    {
        head = GetComponent<EnemyHead>();
        body = this.transform.FindChild("Body");
        enemyBase = GetComponent<EnemyBase>();

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
        if (head.currentState == EnemyHeadState.LOCKED)
        {
            attack();
        }
    }

    private void attack()
    {
        if (weapon == null) return;

        weapon.Fire();
    }
}
