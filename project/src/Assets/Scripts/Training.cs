﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Training : MonoBehaviour
{
    

    
    

    // Use this for initialization
    void Start()
    {
        
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        GameControl.control.level = this.gameObject;

        
    }

    
}
