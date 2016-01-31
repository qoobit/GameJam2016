using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{

    public GameObject CanvasCamera;
    public GameObject Hero;

    public List<GameObject> platforms = new List<GameObject>();
    public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> targets = new List<GameObject>();
    public List<GameObject> portals = new List<GameObject>();


    public void AddTarget(GameObject t)
    {
        if (t != null) targets.Add(t);
    }
    public void AddPlatform(GameObject platform)
    {
        if (platform != null) platforms.Add(platform);
    }

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
