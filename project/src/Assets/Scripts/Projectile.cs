using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, ISpawnable
{
    public Vector3 direction;
    public float speed;
    public float liveTime;
    public float createTime;
    public float damage;
    public GameObject owner; //The entity that fires the weapon
    public Weapon weapon; //The weapon that creates the projectile
    public Spawnable.Type spawnType { get; set; }

    void Start()
    {
        speed = 20.0f;
        liveTime = 2.0f;
        createTime = Time.time;
        direction.Normalize();
    }

    void Update()
    {
        if ((Time.time - createTime) > liveTime)
            GameControl.Destroy(this.gameObject);

        this.gameObject.transform.position += direction * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        IDamageable other = collider.gameObject.GetComponent<IDamageable>();
        if (other != null) other.Hurt(this.damage, this.gameObject);

        //Destroy this projectile
        GameControl.Destroy(this.gameObject);
    }


    void OnCollisionEnter(Collision collision)
    {
        IDamageable other = collision.gameObject.GetComponent<IDamageable>();
        if (other != null) other.Hurt(this.damage, this.gameObject);

        //Destroy this projectile
        GameControl.Destroy(this.gameObject);
    }
}
