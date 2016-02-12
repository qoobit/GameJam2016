using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class Level : MonoBehaviour
{
    public GameObject CanvasCamera;
    public GameObject Hero;
    
    public List<GameObject> projectileList = new List<GameObject>();
    public List<GameObject> portalList = new List<GameObject>();
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

    public void RegisterSpawnable(GameObject gameObject)
    {
        ISpawnable spawnable = gameObject.GetComponent<ISpawnable>();
        if (spawnable != null)
        {
            List<GameObject> spawnableList = getEntityList(spawnable.spawnType);
            if (spawnableList != null)
                spawnableList.Add(gameObject);
        }
    }

    public void UnregisterSpawnable(GameObject gameObject)
    {
        ISpawnable spawnable = gameObject.GetComponent<ISpawnable>();
        if (spawnable != null)
        {
            List<GameObject> spawnableList = getEntityList(spawnable.spawnType);
            if (spawnableList != null)
                spawnableList.Remove(gameObject);
        }
    }

    protected virtual List<GameObject> getEntityList(Spawnable.Type type)
    {
        switch (type)
        {
            case Spawnable.Type.HERO:
                return heroList;

            case Spawnable.Type.PROJECTILE:
                return projectileList;

            case Spawnable.Type.PORTAL:
                return portalList;

            default:
                return null;
        }
    }
}
