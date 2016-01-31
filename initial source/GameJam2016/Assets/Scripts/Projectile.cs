using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed;
    public float lifetime;
    public float age;
    public float damage;
    
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

        Destroy(this.gameObject);
    }
}
