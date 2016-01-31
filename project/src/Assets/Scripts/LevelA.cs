using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelA : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        

   




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
