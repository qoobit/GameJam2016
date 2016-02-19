using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GameControl : MonoBehaviour {
    public bool MusicOn;
    public bool SfxOn;
    public string sceneName;
    public string portalName;
    public int lives;
    public float health;
    public bool dash;
    public bool shoot;
    public static GameControl control;
    public Level level;

    private bool enableMultiDisplay = true;
    private int displayCount = 1;

    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }

        activateMultiDisplays();
    }

    void Start()
    {
        
        //init
        SfxOn = true;
        MusicOn = true;
        //level = null;
        //sceneName = portalName = "";
        lives = 3;
        health = 100f;
        dash = false;
        shoot = false;


        Load();

        createMultiDisplayCameras();
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void Load()
    {
        Debug.Log("LOADING FROM " + Application.persistentDataPath + "/saveData.dat");
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        if (File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);
            UserData data = (UserData)bf.Deserialize(file);
            file.Close();
            
            //loading goes here
            MusicOn = data.MusicOn;
            SfxOn = data.SfxOn;
            sceneName = data.sceneName;
            portalName = data.portalName;
            lives = data.lives;
            health = data.health;
            shoot = data.shoot;
            dash = data.dash;
        }
    }

    public void Save()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("SAVING TO " + Application.persistentDataPath + "/saveData.dat");
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

        UserData data= new UserData();
        data.SfxOn = SfxOn;
        data.MusicOn = MusicOn;
        data.lives = lives;
        data.health = health;
        data.shoot = shoot;
        data.dash = dash;
        data.sceneName = sceneName;
        data.portalName = portalName;

        
        //saving goes here

        bf.Serialize(file, data);
        file.Close();

    }




    private void activateMultiDisplays()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        foreach (string arg in args)
        {
            switch (arg)
            {
                case "-multidisplay":
                    enableMultiDisplay = true;
                    break;
            }
        }
        //GameObject.Find("DD").GetComponent<Text>().text = Display.displays.Length.ToString();
        if (enableMultiDisplay)
        {
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
                displayCount++;
            }
        }
    }

    private void createMultiDisplayCameras()
    {
        
        if (enableMultiDisplay)
        {
            for (int i = 0; i < displayCount; i++)
            {
                UnityEngine.Object cameraObject = Resources.Load("Camera/ThirdPersonCamera", typeof(GameObject));
                GameObject camera = GameObject.Instantiate(cameraObject, Vector3.zero, Quaternion.identity) as GameObject;
                camera.name = "Camera Display " + i.ToString();
                camera.GetComponent<Camera>().targetDisplay = i;

                GameObject hero = GameObject.Find("asset_hero");
                camera.GetComponent<ThirdPersonCamera>().lookAtTarget = hero.transform;
            }
            UnityEngine.VR.VRSettings.showDeviceView = false;
        }
    }

    public static GameObject Spawn(Spawnable.Type type, Vector3 position, Quaternion rotation)
    {
        string resourceString = Spawnable.GetResourceString(type);
        UnityEngine.Object obj = Resources.Load(resourceString, typeof(GameObject));
        GameObject gameObject = GameObject.Instantiate(obj, position, rotation) as GameObject;

        ISpawnable spawnable = gameObject.GetComponent<ISpawnable>();
        if (spawnable == null)
            throw new System.Exception("Unable to register spawnable. No Spawnable component found.");

        spawnable.spawnType = type;
        GameControl.RegisterSpawnable(gameObject);        

        return gameObject;
    }

    public static void Destroy(GameObject gameObject)
    {
        GameControl.UnregisterSpawnable(gameObject);
        GameObject.Destroy(gameObject);
    }

    public static void RegisterSpawnable(GameObject gameObject)
    {
        
        if (GameControl.control.level == null)
            throw new System.Exception("Unable to register spawnable. No instance of Level found");

        GameControl.control.level.RegisterSpawnable(gameObject);
    }

    public static void UnregisterSpawnable(GameObject gameObject)
    {
        if (GameControl.control.level == null)
            throw new System.Exception("Unable to unregister spawnable. No instance of Level found");

        GameControl.control.level.UnregisterSpawnable(gameObject);
    }
}

[Serializable]
class UserData
{
    public bool MusicOn = true;
    public bool SfxOn = true;
    public string sceneName = "";
    public string portalName = "";
    public int lives = 3;
    public float health = 100f;
    public bool dash = false;
    public bool shoot = false;

    
}