using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
    float idleAge;
    float idleDuration;
    int waveCount;
    int waveLimit;
    float waveDuration;
    float waveAge;
    // Use this for initialization
    void Start () {
        GetComponent<Enemy>().weapon.state = WeaponState.GUN;
        GetComponent<Enemy>().weapon.damage = 40f;

        idleAge = 0f;
        idleDuration = 0.1f;
        waveCount = 0;
        waveLimit = 3;
        waveAge=0f;
        waveDuration = 1f;
    }
	
	// Update is called once per frame
	void Update () {
        idleAge += Time.deltaTime;
        if (idleAge >= idleDuration &&waveCount<waveLimit)
        {
            idleAge = 0f;
            Object bulletObject = Resources.Load("Projectile", typeof(GameObject));
            GameObject bullet = Instantiate(bulletObject, gameObject.transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<Projectile>().direction = GetComponent<Enemy>().direction;
            bullet.name = "Turret Projectile";
            waveCount++;
            
        }
        if (waveCount >= waveLimit)
        {
            waveAge += Time.deltaTime;
            if (waveAge >= waveDuration) {
                idleAge = 0f;
                waveAge = 0f;
                waveCount = 0;
            }
            
            
        }
	}
}
