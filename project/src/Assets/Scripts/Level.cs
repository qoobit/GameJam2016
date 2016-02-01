using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{

    public GameObject CanvasCamera;
    public GameObject Hero;

    
    public List<GameObject> bullets = new List<GameObject>();
    
    public List<GameObject> portals = new List<GameObject>();

    

    // Use this for initialization
    void Start()
    {
        if (GameControl.control.portalName != "")
        {
            
            GameObject portal = GameObject.Find(GameControl.control.portalName);
            if (portal != null)
            {
                Hero.transform.position = portal.transform.position;
                Hero.GetComponent<Hero>().facing = portal.GetComponent<Portal>().direction;
                Hero.GetComponent<Hero>().UpdateTransform();
                Hero.GetComponent<Hero>().RealignHMD();
            }
        }
        
        CanvasCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
