using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {
    Object explosionObject;
    // Use this for initialization
    void Start () {
        explosionObject = Resources.Load("Explosion", typeof(GameObject));
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Projectile")
        {
            Instantiate(explosionObject, gameObject.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
    
}
