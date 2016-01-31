using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed;
    public float lifetime;
    public float age;
    public float damage;
    public GameObject owner; //The entity that fires the weapon
    public Weapon weapon; //The weapon that creates the projectile
    
	// Use this for initialization
	void Start () {
        speed = 0.5f;
        age = 0f;
        lifetime = 5f;
        direction.Normalize();
	}
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age > lifetime) Destroy(this.gameObject);
        this.gameObject.transform.position += direction * speed;
	}

    void OnCollisionEnter(Collision collision)
    {
        Damageable other = collision.gameObject.GetComponent<Damageable>();
        other.Hurt(this.damage, this.gameObject);

        //Destroy this projectile
        Destroy(this.gameObject);

        //Tell the weapon that we were destroyed
        if (weapon != null)
            weapon.ProjectileDestroyed();

        
    }
}
