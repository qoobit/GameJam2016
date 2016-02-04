using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform lookAtTarget;
    public float distance = 10.0f;
    public float angleX = 45.0f;
    public float angleY = 0.0f;
    public float angleZ = 0.0f;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (lookAtTarget != null)
        {
            Vector3 cameraDirection = Quaternion.Euler(0f, lookAtTarget.rotation.eulerAngles.y, 0f) * Quaternion.Euler(angleX, angleY, angleZ) * Vector3.forward;
            this.transform.position = lookAtTarget.transform.position + (cameraDirection * -distance);
            this.transform.LookAt(lookAtTarget);
        }
    }
}
