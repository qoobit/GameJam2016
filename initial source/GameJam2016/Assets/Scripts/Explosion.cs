using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    float age;
    float lifetime;
	// Use this for initialization
	void Start () {
        age = 0f;
        lifetime = 999999999f;
	}
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age > lifetime)
        {
            Destroy(gameObject);
        }
	}
}
