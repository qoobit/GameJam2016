using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Training : Level
{
    private GameObject portalEntry;
    private GameObject portalExit;

    override protected void Start()
    {
        base.Start();
        for(int i = 0; i < portalList.Count; i++)
        {
            if(portalList[i].name == "Portal Entry")
            {
                portalEntry = portalList[i];
            }
            else if(portalList[i].name == "Portal Exit")
            {
                portalExit = portalList[i];
                portalExit.SetActive(false);
            }
        }
        
        
    }	
	
	override protected void Update()
    {
        base.Update();
    }

    
}
