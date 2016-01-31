using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class GameControl : MonoBehaviour {
    public string sceneName;
    public string portalName;
    public int lives;
    public bool dash;
    public bool shoot;
    public static GameControl control;
    public GameObject level;


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
    }
	void Start () {

        //init
        sceneName = portalName = "";
        lives = 3;
        dash = false;
        shoot = false;
        Load();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Load()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        if (File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);
            UserData data = (UserData)bf.Deserialize(file);
            file.Close();

            //loading goes here
            sceneName = data.sceneName;
            portalName = data.portalName;
            lives = data.lives;
            shoot = data.shoot;
            dash = data.dash;

        }
    }

    public void Save()
    {
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

        UserData data= new UserData();
        data.lives = lives;
        data.shoot = shoot;
        data.dash = dash;
        data.sceneName = sceneName;
        data.portalName = portalName;
        //saving goes here

        bf.Serialize(file, data);
        file.Close();

    }
}

[Serializable]
class UserData
{
    public string sceneName = "";
    public string portalName = "";
    public int lives = 0;
    public bool dash = false;
    public bool shoot = false;

    
}