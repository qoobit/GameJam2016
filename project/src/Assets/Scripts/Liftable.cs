using UnityEngine;
using System.Collections;

public class Liftable : MonoBehaviour {
    public Vector3 SpawnPosition;
    public Quaternion SpawnRotation;

    // Use this for initialization
    void Start () {
        SpawnPosition = transform.position;
        SpawnRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

