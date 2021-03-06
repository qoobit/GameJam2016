﻿using UnityEngine;
using System.Collections.Generic;

public class TurretCannon : Weapon
{
    public enum Mode { NONE, SINGLE }
    public float clipReloadTime; // Time it takes to reload a clip
    public float bulletReloadTime; // Time it takes to reload 1 bullet within a clip
    private int bulletCount; //Number of rounds fired this wave
    private int clipLimit; // Maximum number of rounds for this wave
    private float nextFireTime; //The earliest time we can fire the next bullet
    public float bulletSpeed;
    public GameObject waypointCollection;

    void Start()
    {
        state = WeaponState.GUN;
        damage = 40f;
        nextFireTime = 0f;
        weaponFireMode = (int)Mode.SINGLE;

        resetFireMode();
    }

    public void resetFireMode()
    {
        switch (weaponFireMode)
        {
            case (int)Mode.SINGLE:
                clipReloadTime = 2f;
                bulletReloadTime = 0.5f;
                bulletCount = 0;
                clipLimit = 2;
                bulletSpeed = 40f;
                break;
        }

    }

    public override void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        switch (weaponFireMode)
        {
            case (int)Mode.SINGLE:
                fireSingle();
                break;
        }
    }


    private void createProjectile(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        GameObject turretGameObj = GameControl.Spawn(Spawnable.Type.ENEMY_TURRET, position, rotation);
        turretGameObj.name = "Boss Turret Minion";
        turretGameObj.layer = this.transform.parent.gameObject.layer;

        Turret turret = turretGameObj.GetComponent<Turret>();
        turret.setCurrentState((int)Turret.State.PROJECTILE);

        Rigidbody rigidBody = turretGameObj.GetComponent<Rigidbody>();
        rigidBody.velocity = direction * bulletSpeed;

        EnemyWalk turretEnemyWalk = turretGameObj.GetComponent<EnemyWalk>();
        turretEnemyWalk.state = EnemyWalk.State.IDLE;
        turretEnemyWalk.waypointCollection = waypointCollection;
    }

    public void fireSingle()
    {
        createProjectile(this.transform.position, this.transform.rotation, this.transform.forward);

        bulletCount++;
        if (bulletCount < clipLimit)
        {
            nextFireTime = Time.time + bulletReloadTime;
        }
        else if (bulletCount >= clipLimit)
        {
            bulletCount = 0;
            nextFireTime = Time.time + clipReloadTime;
        }
    }
}
