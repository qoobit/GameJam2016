using UnityEngine;
using System.Collections;

public class QoobitOVR : MonoBehaviour
{
    public GameObject Reticle;
    public GameObject Recticle2D;
    public GameObject FocusObject;
    GameObject LastFocusObject;
    public bool SearchForObject;
    

    // Use this for initialization
    void Start()
    {
        FocusObject = null;
    }

    public bool Discovered()
    {
        return (FocusObject != null && LastFocusObject == null);
    }
    public bool Lost()
    {
        return (FocusObject == null && LastFocusObject != null);
    }
    // Update is called once per frame
    void Update()
    {
        Reticle.SetActive(false);
        FocusObject = null;
        if (SearchForObject)
        {
            LayerMask layerMask = new LayerMask();
            layerMask = (1 << LayerMask.NameToLayer("Targets"));
            layerMask |= (1 << LayerMask.NameToLayer("Enemy"));

            /*
            if (hit.collider.gameObject.GetComponent<MeshRenderer>() != null)
            {
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = greenMat;
            }

            SetSkinnedMeshRendererColor(hit.collider.gameObject, new Color(0, 255f/255f, 86f/255f,0f));
            */
           
            RaycastHit hit;

            if (Physics.SphereCast(Camera.main.transform.position, 2f, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Reticle.SetActive(true);
                FocusObject = hit.collider.gameObject;

                Vector3 targetCenter = FocusObject.transform.TransformPoint(FocusObject.GetComponent<CapsuleCollider>().center);
                Vector3 directionToCenter = targetCenter - Camera.main.transform.position;
                directionToCenter.Normalize();
                Reticle.transform.position = targetCenter - (directionToCenter * 3f);
                

            }
        }

        LastFocusObject = FocusObject;
    }

    public void Realign(Vector3 position, Transform lookAt)
    {
        transform.position = position;
        transform.LookAt(lookAt);
    }
}
