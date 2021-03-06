﻿using UnityEngine;
using System.Collections;

public class Enemy : StateEntity, IDamageable, ISpawnable
{
    [Header("Enemy Status")]
    public float hp = 100f;
    public float baseDamage = 0f;
    public Spawnable.Type spawnType { get; set; }

    public Guage guage;
    protected Object explosionObject;
    protected Weapon weapon;
    
    override protected void Start()
    {
        base.Start();
        guage = new Guage();
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
    }


    override protected void Update()
    {
        base.Update();

        hp = guage.value;
	}

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (baseDamage == 0) return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Hurt(baseDamage, this.gameObject);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (baseDamage == 0) return;
        
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.Hurt(baseDamage, this.gameObject);
    }

    public void Explode()
    {
        Instantiate(explosionObject, gameObject.transform.position, Quaternion.identity);
        GameControl.Destroy(this.gameObject);
    }

    public virtual void Hurt(float damage, GameObject attacker)
    {
        guage.value -= damage;
        
        if (guage.value <= 0f)
        {
            if (attacker.name == "Hero" && (attacker.GetComponent<Hero>().IsDashing))
            {
                Explode();
            }
            else
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        baseDamage = 0f;
        if (GetComponent<EnemyWalk>() != null) GetComponent<EnemyWalk>().enabled = false;
        if (GetComponent<EnemyHead>() != null) GetComponent<EnemyHead>().enabled = false;
        if (GetComponent<CapsuleCollider>() != null) GetComponent<CapsuleCollider>().enabled = false;
        if (GetComponent<NavMeshAgent>() != null) GetComponent<NavMeshAgent>().enabled = false;
        StartCoroutine(WaitAndExplode(0.5f));
    }

    protected IEnumerator WaitAndExplode(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        this.Explode();
    }

    public bool isAlive()
    {
        return (guage.value > 0);
    }
}
