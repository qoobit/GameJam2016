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
    public GameObject level;

    private bool enableMultiDisplay = true;
    private int displayCount = 1;

    // Use this for initialization
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
    /*
	void Start () {
        //init
        SfxOn = true;
        MusicOn = true;
        level = null;
        sceneName = portalName = "";
        lives = 3;
        health = 100f;
        dash = false;
        shoot = false;
        

        Load();

        createMultiDisplayCameras();


	}
    */
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        //init
        SfxOn = true;
        MusicOn = true;
        level = null;
        sceneName = portalName = "";
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