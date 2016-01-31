using UnityEngine;
using System.Collections;

public class Turret : Damageable {
    public Weapon weapon;

    // Use this for initialization
    void Start ()
    {
        //Load a blaster as our weapon
        Object blasterObject = Resources.Load("Weapons/Blaster", typeof(GameObject));
        GameObject blasterWeapon = GameObject.Instantiate(blasterObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        blasterWeapon.transform.parent = this.transform;
        weapon = blasterWeapon.GetComponent<Weapon>();
    }
	
	// Update is called once per frame
	void Update ()
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
