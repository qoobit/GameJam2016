using UnityEngine;
using System.Collections;

public class Enemy : Damageable
{
    public float hp = 100f;
    public float baseDamage = 0f;

    protected Guage guage;
    protected Object explosionObject;
    protected Weapon weapon;

    
    void Start ()
    {
        guage = new Guage();
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
    }
	

	void Update () {
        hp = guage.value;
	}


    void OnTriggerStay(Collider other)
    {
        if (baseDamage == 0) return;
        
        Damageable damageable = other.gameObject.GetComponent<Damageable>();
        if (damageable != null)
            damageable.Hurt(baseDamage, this.gameObject);
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
        StartCoroutine(WaitAndExplode(0.5f));
    }

    protected IEnumerator WaitAndExplode(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.Explode();
    }
}
