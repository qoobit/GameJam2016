using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

    public Color WaypointColor = Color.gray;
    public float Size = 0.5f;


	void Start () {
	
	}
	
	
	void Update () {
	
	}

    void OnDrawGizmos()
    {
        Gizmos.color = WaypointColor;
        Gizmos.DrawSphere(transform.position, Size);
    }
}
