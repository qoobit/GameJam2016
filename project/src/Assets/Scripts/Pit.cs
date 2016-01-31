using UnityEngine;
using System.Collections;

public class Pit : MonoBehaviour {

	// Use this for initialization
	void Start () {

      
    }
	
	// Update is called once per frame
	void Update () {
        GetComponent<Enemy>().baseDamage = 100f;
    }
}
