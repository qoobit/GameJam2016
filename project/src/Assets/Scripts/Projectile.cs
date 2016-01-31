using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed;
    public float liveTime;
    public float createTime;
    public float damage;
    public GameObject owner; //The entity that fires the weapon
    public Weapon weapon; //The weapon that creates the projectile
    
	// Use this for initialization
	void Start () {
        speed = 20.0f;
        liveTime = 2.0f;
        createTime = Time.time;
        direction.Normalize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if ((Time.time - createTime) > liveTime)
            Destroy(this.gameObject);

        this.gameObject.transform.position += direction * (speed * Time.deltaTime);
	}

    void OnCollisionEnter(Collision collision)
    {
        Damageable other = collision.gameObject.GetComponent<Damageable>();
        other.Hurt(this.damage, this.gameObject);

        //Destroy this projectile
        Destroy(this.gameObject);
    }
}
