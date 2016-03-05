using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class Level : MonoBehaviour
{
    private AudioSource BackgroundAudio;

    public AudioClip BackgroundAudioClip;
    public GameObject CanvasCamera;
    public GameObject VRCamera;
    public GameObject Hero;
    
    public List<GameObject> projectileList = new List<GameObject>();
    public List<GameObject> portalList = new List<GameObject>();
    public List<GameObject> heroList = new List<GameObject>();

   

    private float listCleanupTime = 5f;
    private float nextCleanupTime = 10f;


    protected virtual void Start()
    {

        GameControl.control.level = this;

        BackgroundAudio = gameObject.AddComponent<AudioSource>();
        BackgroundAudio.clip = BackgroundAudioClip;
        BackgroundAudio.loop = true;
        PlayBGM();

        //Spawn Hero and Assign VR Camera
        Object heroObject = Resources.Load("Hero", typeof(GameObject));
        Hero = GameObject.Instantiate(heroObject, Vector3.zero, Quaternion.identity) as GameObject;
        Hero.name = "Hero";
        Hero.transform.SetParent(transform);
        Hero.GetComponent<Hero>().hmdRig = Camera.main.transform.parent.gameObject;


        if (GameControl.control.portalName != "")
        {
            GameObject portal = GameObject.Find(GameControl.control.portalName);
            if (portal != null)
            {
                Hero.transform.position = portal.transform.position;
                Hero.GetComponent<Hero>().facing = portal.GetComponent<Portal>().Direction;
                Hero.GetComponent<Hero>().UpdateTransform();


                // Currently Level handles the initial camera alignment - should this be moved to portal?
                VRCamera.GetComponent<QoobitOVR>().FollowObject = Hero;
                VRCamera.GetComponent<QoobitOVR>().Realign(Hero.transform.position,Hero.GetComponent<Hero>().facing);
            }
        }

        heroList.Add(Hero);
        CanvasCamera.SetActive(true);
    }

    protected virtual void Update()
    {
        if (Time.time >= nextCleanupTime)
        {
            listCleanup();
            nextCleanupTime = Time.time + listCleanupTime;
        }
    }

    protected virtual void listCleanup()
    {
        //Remove any null pointers from our list
        List<GameObject> newProjectileList = new List<GameObject>();
        for(int i=0; i< projectileList.Count; i++)
        {
            if (projectileList[i] != null)
                newProjectileList.Add(projectileList[i]);
        }
        projectileList = newProjectileList;

        List<GameObject> newPortalList = new List<GameObject>();
        for (int i = 0; i < portalList.Count; i++)
        {
            if (portalList[i] != null)
                newPortalList.Add(portalList[i]);
        }
        portalList = newPortalList;

        List<GameObject> newHeroList = new List<GameObject>();
        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i] != null)
                newHeroList.Add(heroList[i]);
        }
        heroList = newHeroList;
    }

    public void PlayBGM()
    {
        BackgroundAudio.Play();
    }
    public void SetBGMVolume(float volume)
    {
        BackgroundAudio.volume = volume;
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
