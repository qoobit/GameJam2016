using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaLevel : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GetComponent<Level>().AddPlatform(GameObject.Find("Floor"));
        GetComponent<Level>().AddTarget(GameObject.Find("Pit"));
    }
	
	// Update is called once per frame
	void Update () {
        GameControl.control.level = this.gameObject;
    }
}
