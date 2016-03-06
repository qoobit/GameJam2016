using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DetectionMode { RAYCAST, SPHERECAST}
public enum HMDModel { NONE, OCULUS_RIFT_DK2, OCULUS_RIFT_DK1, HTC_VIVE_PRE, HTC_VIVE, OSVR}
public enum FollowMode { NONE, DIRECT };
public class QoobitOVR : MonoBehaviour
{
    public DetectionMode DetectionMode;
    public GameObject Reticle;
    public GameObject Reticle2D;
    public GameObject FocusObject;
    public Vector3 LookAt;
    public Color HighlightColor;

    
    public bool SearchForObject = true;

    //custom camera motion variables
    public GameObject FollowObject;
    public FollowMode FollowMode;
    public Vector3 RelativePosition;


    public HMDModel HmdModel;
    public List<GameObject> DetectionList;

    public LayerMask DetectionLayerMask;

    GameObject PrevFocusObject;

    public float hoverDuration;

    public float SphereRadius;

    public bool ShowReticle;
    public bool ShowReticle2D;

    public Color GizmosForwardColor;
    public Color GizmosLookAtColor;
    public bool ShowHmdReferences;

    public float fovH;
    public float fovV;
    public float some3d;
    public float comfort;
    public float maxHeadTurn;

    public float upComfort;
    public float downComfort;
    public float upHeadMax;
    public float downHeadMax;

    // Use this for initialization
    void Start()
    {
        DetectionList = new List<GameObject>();
    }

    

    public bool Discovered()
    {
        return (FocusObject != null && PrevFocusObject == null);
    }
    public bool Lost()
    {
        return (FocusObject == null && PrevFocusObject != null);
    }
    
    void SetFocusObject(GameObject g)
    {
        //item found
        if (ShowReticle) {
            Reticle.SetActive(true);
        }
        
        FocusObject = g;
        if (FocusObject == PrevFocusObject)
        {
            hoverDuration += Time.deltaTime;
        }
        
        Reticle2D.GetComponent<Animator>().SetBool("found", true);

        Vector3 targetCenter = Vector3.zero;
        if (FocusObject.GetComponent<CapsuleCollider>())
        {
            targetCenter = FocusObject.transform.TransformPoint(FocusObject.GetComponent<CapsuleCollider>().center);
        }
        if (FocusObject.GetComponent<BoxCollider>())
        {
            targetCenter = FocusObject.transform.TransformPoint(FocusObject.GetComponent<BoxCollider>().center);
        }

        Vector3 directionToCenter = targetCenter - Camera.main.transform.position;
        directionToCenter.Normalize();
        Reticle.transform.position = targetCenter - (directionToCenter * 3f);
    }


    void SetGameObjectColor(GameObject go, Color c) {
        
        if (go.GetComponent<MeshRenderer>() != null)
        {

            go.GetComponent<MeshRenderer>().material.SetFloat("_Mode", 2);
            go.GetComponent<MeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            go.GetComponent<MeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //go.GetComponent<MeshRenderer>().material.SetInt("_ZWrite", 0);
            //go.GetComponent<MeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
            //go.GetComponent<MeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
            go.GetComponent<MeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //go.GetComponent<MeshRenderer>().material.renderQueue = 3000;

            go.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
        }
        else if (go.GetComponent<SkinnedMeshRenderer>() != null)
        {
            go.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 2);
            go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 0);
            //go.GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
            //go.GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
            go.GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //go.GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3000;

            go.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", c);
        }
        else if (go.GetComponent<SpriteRenderer>() != null)
        {
            go.GetComponent<SpriteRenderer>().color = c;
        
        }

        foreach (Transform child in go.transform)
        {
            SetGameObjectColor(child.gameObject, c);
        }

    }

    void RigFollow()
    {
        if (FollowObject == null) return;

        //camera motion
        switch (FollowMode)
        {
            case FollowMode.DIRECT:
                Realign(FollowObject.transform.position, Vector3.zero);
                break;
            case FollowMode.NONE:
            default:
                break;
        }

    }
    void Update()
    {
        RigFollow();

        //reticle stuff
        Reticle.SetActive(false);

        if (ShowReticle2D)
        {
            Reticle2D.SetActive(true);
            Reticle2D.GetComponent<Animator>().SetBool("found", false);
        }
        else
        {
            Reticle2D.SetActive(false);
        }
        
        if (FocusObject != null)
        {
            SetGameObjectColor(FocusObject, Color.white);
        }
        
        FocusObject = null;

        if (SearchForObject)
        { 
            DetectionList = new List<GameObject>();

            
            RaycastHit[] hits;

            switch (DetectionMode)
            {
                
                case DetectionMode.SPHERECAST:

              
                    hits = Physics.SphereCastAll(Camera.main.transform.position, SphereRadius, Camera.main.transform.forward, Mathf.Infinity, DetectionLayerMask);
                    if (hits.Length > 0)
                    {
                        //set closest object
                        SetFocusObject(hits[0].collider.gameObject);
                    }
                    for(int i=0; i < hits.Length; i++)
                    {
                        DetectionList.Add(hits[i].collider.gameObject);
                    }
                    break;
                case DetectionMode.RAYCAST:
                default:
                   
                    hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, Mathf.Infinity, DetectionLayerMask);
                    if (hits.Length > 0)
                    {
                        SetFocusObject(hits[0].collider.gameObject);
                    }
                    for (int i = 0; i < hits.Length; i++)
                    {
                        DetectionList.Add(hits[i].collider.gameObject);
                    }
                    break;
            }
            if (FocusObject != null)
            {
                
                SetGameObjectColor(FocusObject, HighlightColor);
            }
            
        }

        //post detection
        if (FocusObject!=PrevFocusObject)
        {
            hoverDuration = 0f;
        }

        PrevFocusObject = FocusObject;
        
    }

   
    public void Realign(Vector3 targetPosition, Vector3 forward)
    {
        Debug.Log("A"+targetPosition+" "+forward);
        Vector3 position;
        Vector3 lookAt = Vector3.zero;

        position = targetPosition;

        if (forward != Vector3.zero)
        {
            position += (forward * RelativePosition.z);

            lookAt = position + (forward * 1000f);
            LookAt = lookAt;
            Debug.Log("RESET");
        }
        else
        {
            //derive forward from look at
            Vector3 proxyForward = LookAt - transform.position;
            proxyForward.Normalize();
            position += (proxyForward * RelativePosition.z);
        }
        position += new Vector3(0, RelativePosition.y, 0f);
        
        Debug.Log(targetPosition+" "+position + " " + lookAt);

        transform.position = position;
        
        if (lookAt != Vector3.zero) {
            GameObject temp = new GameObject();
            temp.transform.position = LookAt;
            transform.LookAt(temp.transform);
            Destroy(temp);
        }
        

    }



    void OnDrawGizmos()
    {

        Gizmos.color = GizmosForwardColor;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * Camera.main.farClipPlane));

        if (LookAt != null)
        {
            Vector3 directionToFocal = LookAt - transform.position;
            directionToFocal.Normalize();
            Gizmos.color = GizmosLookAtColor;
            Gizmos.DrawLine(transform.position, transform.position + (directionToFocal * Camera.main.farClipPlane));
        }


        if (ShowHmdReferences)
        {
            // Draw zones


            fovV = 0f;
            fovH = 0f;
            float crossEyed;
            float eyeStrain;
            float strong3d;
            
            fovV = 60f;
            crossEyed = 0.3f;
            eyeStrain = 0.5f;
            strong3d = 10f;
            some3d = 20f;



            switch (HmdModel)
            {
                case HMDModel.OCULUS_RIFT_DK2:
                    fovV = 106.1888f;
                    fovH = 94.16f;
                    break;
                default:
                    fovV = 60f;
                    break;
            }


            comfort = 60f;
            maxHeadTurn = 110f;
            upComfort = 20f;
            downComfort = 12f;
            upHeadMax = 60f;
            downHeadMax = 40f;

            float contentAngle = fovH + comfort;
            float peripheralAngle = fovH + maxHeadTurn;

            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawSphere(transform.position, crossEyed);
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(transform.position, eyeStrain);

            Gizmos.color = new Color(0f, 0.5f, 0f, 0.5f);
            Gizmos.DrawSphere(transform.position, strong3d);

            //outer some 3d zone
            //Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.5f);
            //Gizmos.DrawSphere(transform.position, some3d);


            Gizmos.matrix = transform.localToWorldMatrix;


            Vector3 leftLimit;
            Vector3 rightLimit;
            Vector3 upLimit;
            Vector3 downLimit;


            //hmd base view zone
            Gizmos.color = Color.red;
            Vector3 flatPoint = Vector3.forward * some3d;
            rightLimit = Quaternion.Euler(0, fovH / 2f, 0) * flatPoint;
            leftLimit = Quaternion.Euler(0, -fovH / 2f, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, rightLimit);
            Gizmos.DrawLine(Vector3.zero, leftLimit);
            

            //head rotate comfort zone
            Gizmos.color = new Color(1f, 1f, 0f);
            rightLimit = Quaternion.Euler(0, comfort / 2f, 0) * flatPoint;
            leftLimit = Quaternion.Euler(0, -comfort / 2f, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, rightLimit);
            Gizmos.DrawLine(Vector3.zero, leftLimit);

            upLimit = Quaternion.Euler(-upComfort, 0, 0) * flatPoint;
            downLimit = Quaternion.Euler(downComfort, 0, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, upLimit);
            Gizmos.DrawLine(Vector3.zero, downLimit);


            //max head rotate zone
            Gizmos.color = new Color(0f, 1f, 1f);
            rightLimit = Quaternion.Euler(0, maxHeadTurn / 2f, 0) * flatPoint;
            leftLimit = Quaternion.Euler(0, -maxHeadTurn / 2f, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, rightLimit);
            Gizmos.DrawLine(Vector3.zero, leftLimit);

            upLimit = Quaternion.Euler(-upHeadMax, 0, 0) * flatPoint;
            downLimit = Quaternion.Euler(downHeadMax, 0, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, upLimit);
            Gizmos.DrawLine(Vector3.zero, downLimit);


            //peripheral zone
            Gizmos.color = new Color(1f, 0f, 1f);
            rightLimit = Quaternion.Euler(0, contentAngle / 2f, 0) * flatPoint;
            leftLimit = Quaternion.Euler(0, -contentAngle / 2f, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, rightLimit);
            Gizmos.DrawLine(Vector3.zero, leftLimit);
            rightLimit = Quaternion.Euler(0, peripheralAngle / 2f, 0) * flatPoint;
            leftLimit = Quaternion.Euler(0, -peripheralAngle / 2f, 0) * flatPoint;
            Gizmos.DrawLine(Vector3.zero, rightLimit);
            Gizmos.DrawLine(Vector3.zero, leftLimit);
        }
       
    }

}
