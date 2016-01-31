using UnityEngine;
using System.Collections;

public class Enemy : Damageable
{
    public float hp;
    Guage guage;
    public float baseDamage;
    Object explosionObject;
    public Weapon weapon;
    public Vector3 direction;

    
    void Start () {
        guage = new Guage();
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
        hp = 100f;
        baseDamage = 40f;
        direction = new Vector3(0, 0, 1);
    }
	

	void Update () {
        hp = guage.value;
	}

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionObject, gameObject.transform.position, Quaternion.identity) as GameObject;

        Destroy(this.gameObject);
    }

    override public void Hurt(float damage, GameObject attacker)
    {
        guage.value -= damage;
        if (hp <= 0f)
        {
            Explode();
        }
    }

    override public void Die()
    {
        this.Explode();
    }
}
