using UnityEngine;
using System.Collections;

public class DebugInputs : MonoBehaviour
{
    private Vector3 mouseOrigin;
    private bool mouseOriginValid = false;
    private float moveSpeedDefault = 5.0f;
    private float moveSpeedMultiplier = 10.0f;
    private float rotateSpeed = 20.0f;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	
        if (Input.GetMouseButton(1))
        {
            float moveSpeed = moveSpeedDefault;

            // Camera movement

            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction.z += 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction.x -= 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction.z -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction.x += 1;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction.y -= 1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction.y += 1;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed *= moveSpeedMultiplier;
            }
            direction.Normalize();
            
            this.transform.position += (this.transform.rotation * direction * moveSpeed * Time.deltaTime);


            // Camera rotation
            float xRotate = -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
            float yRotate = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            Quaternion xRotation = Quaternion.Euler(xRotate, 0f, 0f);
            Quaternion yRotation = Quaternion.Euler(0f, yRotate, 0f);

            //Look left and right but only rotate about the world Y axis
            Quaternion resetXRotation = Quaternion.Euler(-this.transform.rotation.eulerAngles.x, 0f, 0f);
            this.transform.rotation *= resetXRotation * yRotation * Quaternion.Inverse(resetXRotation);

            //Look up and down
            this.transform.rotation *= xRotation;
        }
	}
}
