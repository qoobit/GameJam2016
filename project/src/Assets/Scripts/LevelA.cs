using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelA : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        

        GetComponent<Level>().AddPlatform(GameObject.Find("platform"));
        GetComponent<Level>().AddPlatform(GameObject.Find("Plane"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (1)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (2)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (3)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (4)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (5)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("platform (6)"));
        GetComponent<Level>().AddPlatform(GameObject.Find("level_one_piece01_start"));
        GetComponent<Level>().AddPlatform(GameObject.Find("level_one_piece02_mid"));
        GetComponent<Level>().AddPlatform(GameObject.Find("level_one_piece03_end"));
        GetComponent<Level>().AddPlatform(GameObject.Find("Nudger A"));
        GetComponent<Level>().AddPlatform(GameObject.Find("Nudger B"));
        GetComponent<Level>().AddPlatform(GameObject.Find("Nudger C"));

        GetComponent<Level>().AddTarget(GameObject.Find("Enemy"));
        GetComponent<Level>().AddTarget(GameObject.Find("Pit"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (1)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (2)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (3)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (4)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (5)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (6)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (7)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (8)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (9)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Target (10)"));

        GetComponent<Level>().AddTarget(GameObject.Find("Turret"));
        GetComponent<Level>().AddTarget(GameObject.Find("Turret (1)"));
        GetComponent<Level>().AddTarget(GameObject.Find("Turret (2)"));

        
    }
	
	// Update is called once per frame
	void Update () {
        GameControl.control.level = this.gameObject;
    }
}
