using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public float hp;
    Guage guage;
    public float baseDamage;
    Object explosionObject;
    public Weapon weapon;
    public Vector3 direction;

    
    void Start () {
        guage = new Guage();
        weapon = new Weapon();
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
        hp = 100f;
        baseDamage = 40f;
        direction = new Vector3(0, 0, 1);
    }
	

	void Update () {
        hp = guage.value;
	}
    void OnTriggerEnter(Collider other)
    {
        
        if (other.name == "Hero Projectile")
        {
            
            guage.value -= other.gameObject.GetComponent<Projectile>().weapon.damage;
            Destroy(other.gameObject);
            if (hp <= 0f) {
                Explode();  
            }
        }
    }

    public void Explode()
    {
        GameObject explosion = Instantiate(explosionObject, gameObject.transform.position, Quaternion.identity) as GameObject;

        Destroy(this.gameObject);
    }
    
}
