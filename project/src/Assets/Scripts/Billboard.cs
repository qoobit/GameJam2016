using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {
    public GameObject Camera;

    public bool ConstrainX;
    public bool ConstrainY;
    public bool ConstrainZ;

    public bool DebugVisible;
    public Color LineColor;
    
    void Start () {
	}
	
	
	void Update () {
        if (Camera != null) {
            
            if (Camera != null) {
                GameObject lookAt = new GameObject();
                try {
                    lookAt.transform.SetParent(Camera.transform);
                    lookAt.transform.localPosition = Vector3.zero;
                    //lookAt.transform.localEulerAngles = Vector3.zero;
                    //lookAt.transform.localScale = Vector3.one;
                    lookAt.transform.SetParent(Camera.transform.parent);
                    
                    if (ConstrainX)
                    {
                        lookAt.transform.position = new Vector3(transform.position.x, lookAt.transform.position.y, lookAt.transform.position.z);
                    }
                    if (ConstrainY)
                    {
                        lookAt.transform.position = new Vector3(lookAt.transform.position.x, transform.position.y, lookAt.transform.position.z);
                    }
                    if (ConstrainZ)
                    {
                        lookAt.transform.position = new Vector3(lookAt.transform.position.x, lookAt.transform.position.y, transform.position.z);
                    }


                    transform.LookAt(lookAt.transform);

                    if (DebugVisible) {
                        Vector3 directionToHMD = Camera.transform.position - transform.position;
                        float lineLength = directionToHMD.magnitude;
                        directionToHMD.Normalize();

                        Debug.DrawLine(transform.position, transform.position + (directionToHMD * lineLength), LineColor);
                    }
                }
                catch
                {
                    DestroyImmediate(lookAt);
                }
                DestroyImmediate(lookAt);


            }
            
        }
    }
}
