using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndOfTimeLevel : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        GetComponent<Level>().AddPlatform(GameObject.Find("level_one_piece01_start"));
        GetComponent<Level>().AddTarget(GameObject.Find("Pit"));
    }
	
	// Update is called once per frame
	void Update () {
        GameControl.control.level = this.gameObject;
    }
}
