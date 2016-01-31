using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    private float clipReloadTime; // Time it takes to reload a clip
    private float bulletReloadTime; // Time it takes to reload 1 bullet within a clip
    private int bulletCount; //Number of rounds fired this wave
    private int clipLimit; // Maximum number of rounds for this wave
    private float nextFireTime; //The earliest time we can fire the next bullet

    // Use this for initialization
    void Start ()
    {
        state = WeaponState.GUN;
        damage = 40f;
        nextFireTime = 0f;
        weaponFireMode = 0;

        reset();

    }
	
	// Update is called once per frame
	void Update () {
    }

    public void reset()
    {
        switch (weaponFireMode)
        {
            case 0:
                clipReloadTime = 1f;
                bulletReloadTime = 0.1f;
                bulletCount = 0;
                clipLimit = 3;
                break;

            case 1:
                clipReloadTime = 1f;
                bulletReloadTime = 0.1f;
                bulletCount = 0;
                clipLimit = 3;
                break;
        }
        
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime)
            return;

        Debug.Log("Firing at " + Time.time.ToString());
        switch (weaponFireMode)
        {
            case 0: fireOne(); break;
            case 1: fireRing(); break;
        }
    }


    private void createProjectile(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        Object bulletObject = Resources.Load("Projectile", typeof(GameObject));
        GameObject bullet = GameObject.Instantiate(bulletObject, position, rotation) as GameObject;
        bullet.name = "Blaster Projectile";

        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.direction = direction;
        projectile.owner = this.transform.parent.gameObject;
    }

    public override void ProjectileDestroyed()
    {
        if (bulletCount > 0) bulletCount--;
    }

    public void fireOne()
    {
        createProjectile(gameObject.transform.position, this.transform.rotation, this.transform.forward);   
            
        bulletCount++;
        if (bulletCount < clipLimit)
        {
            nextFireTime = Time.time + bulletReloadTime;
        }
        else if (bulletCount >= clipLimit)
        {
            nextFireTime = Time.time + clipReloadTime;
        }
    }

    public void fireRing()
    {

    }
}
