using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed;
    public float lifetime;
    public float age;
	// Use this for initialization
	void Start () {
        speed = 0.5f;
        age = 0f;
        lifetime = 5f;
	}
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age > lifetime) Destroy(this.gameObject);
        this.gameObject.transform.position += direction * speed;
	}
}
