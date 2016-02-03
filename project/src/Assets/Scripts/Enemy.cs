using UnityEngine;
using System.Collections;

public class Enemy : Damageable
{
    public float hp = 100f;
    public float baseDamage = 0f;

    public Guage guage;
    protected Object explosionObject;
    protected Weapon weapon;

    
    protected virtual void Start ()
    {
        guage = new Guage();
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
    }


    protected virtual void Update () {
        hp = guage.value;
	}



    void OnCollisionEnter(Collision collision)
    {
        if (baseDamage == 0) return;

        Damageable damageable = collision.gameObject.GetComponent<Damageable>();
        if (damageable != null)
            damageable.Hurt(baseDamage, this.gameObject);
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
        
        if (guage.value <= 0f)
        {
            if (attacker.name == "Hero" && (attacker.GetComponent<Hero>().IsDashing()))
            {
                Explode();
            }
            else
            {
                Die();
            }
            
        }
    }

    override public void Die()
    {
        baseDamage = 0f;
        if(GetComponent<EnemyBase>()!=null) GetComponent<EnemyBase>().CurrentState = EnemyBaseState.IDLE;
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
