﻿using UnityEngine;
using System.Collections;

public class Cannon : Enemy {

    // Use this for initialization
    protected override void Start ()
    {
        //Load a blaster as our weapon
        Object blasterObject = Resources.Load("Weapons/Blaster", typeof(GameObject));
        GameObject blasterWeapon = GameObject.Instantiate(blasterObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        blasterWeapon.transform.parent = this.transform;
        weapon = blasterWeapon.GetComponent<Weapon>();
        weapon.weaponFireMode = 1;
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if (this.weapon != null)
            this.weapon.Fire();
    }

    override public void Hurt(float damage, GameObject attacker)
    {
        Debug.Log(this.GetType().ToString() + " Hurt not implemented");
    }

    override public void Die()
    {
        Debug.Log(this.GetType().ToString() + " Die not implemented");
    }
}
