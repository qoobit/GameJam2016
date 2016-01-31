using UnityEngine;
using System.Collections;

public class Blaster : Weapon
{
    private float idleAge;
    private float idleDuration;
    private int waveCount;
    private int waveLimit;
    private float waveDuration;
    private float waveAge;

    // Use this for initialization
    void Start ()
    {
        state = WeaponState.GUN;
        damage = 40f;

        idleAge = 0f;
        idleDuration = 0.1f;
        waveCount = 0;
        waveLimit = 3;
        waveAge = 0f;
        waveDuration = 1f;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Fire()
    {
        idleAge += Time.deltaTime;
        if (idleAge >= idleDuration && waveCount < waveLimit)
        {
            idleAge = 0f;
            
            Object bulletObject = Resources.Load("Projectile", typeof(GameObject));
            GameObject bullet = GameObject.Instantiate(bulletObject, gameObject.transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<Projectile>().direction = this.transform.forward;
            bullet.name = "Blaster Projectile";
            waveCount++;

        }
        if (waveCount >= waveLimit)
        {
            waveAge += Time.deltaTime;
            if (waveAge >= waveDuration)
            {
                idleAge = 0f;
                waveAge = 0f;
                waveCount = 0;
            }
        }
    }
}
