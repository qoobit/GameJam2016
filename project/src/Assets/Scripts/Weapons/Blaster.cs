using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    private float clipReloadTime; // Time it takes to reload a clip
    private float bulletReloadTime; // Time it takes to reload 1 bullet within a clip
    private int bulletCount; //Number of rounds fired this wave
    private int clipLimit; // Maximum number of rounds for this wave
    private float nextFireTime; //The earliest time we can fire the next bullet
    private float bulletSpeed;
    private float bulletLiveTime;
    // Use this for initialization
    void Start ()
    {
        state = WeaponState.GUN;
        damage = 40f;
        nextFireTime = 0f;
        weaponFireMode = 1;

        resetFireMode();

    }
	
	// Update is called once per frame
	void Update () {
    }

    public void resetFireMode()
    {
        switch (weaponFireMode)
        {
            case 0: //burst fire 3 shots
                clipReloadTime = 1f;
                bulletReloadTime = 0.1f;
                bulletCount = 0;
                clipLimit = 3;
                bulletSpeed = 40f;
                bulletLiveTime = 1f;
                break;

            case 1: // Ring
                clipReloadTime = 2f;
                bulletReloadTime = 0.5f; //Not used
                bulletCount = 0; //Not used
                clipLimit = 3; //Not used
                bulletSpeed = 30f;
                bulletLiveTime = 2f;
                break;
        }
        
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime)
            return;

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
        projectile.speed = bulletSpeed;
        projectile.liveTime = bulletLiveTime;
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
            bulletCount = 0;
            nextFireTime = Time.time + clipReloadTime;
        }
    }

    public void fireRing()
    {
        int numBullets = 5;
        float radius = 1f;
        Vector3 drift = new Vector3(0f, 0.1f, 0f);

        float maxDegrees = 360f;
        float offsetAngle = maxDegrees / numBullets;
        for (int i = 0; i < numBullets; i++)
        {
            Vector3 offset = Quaternion.AngleAxis(offsetAngle * i, this.transform.forward) * this.transform.up * radius;
            Vector3 position = this.transform.position + offset;
            Vector3 driftOffset = Quaternion.AngleAxis(offsetAngle * i, this.transform.forward) * drift;
            createProjectile(position, this.transform.rotation, this.transform.forward + driftOffset);
        }

        nextFireTime = Time.time + clipReloadTime;
    }
}
