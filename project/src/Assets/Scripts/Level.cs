using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class Level : MonoBehaviour
{
    public GameObject CanvasCamera;
    public GameObject Hero;
    
    public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> portals = new List<GameObject>();
    public List<GameObject> heroList = new List<GameObject>();

    protected virtual void Start()
    {
        GameControl.control.level = this;

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

        heroList.Add(Hero);
        CanvasCamera.SetActive(true);
    }

    protected virtual void Update()
    {

    }

    public List<GameObject> GetHeroList()
    {
        return this.heroList;
    }
}
