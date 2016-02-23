using UnityEngine;
using System.Collections;

public class Guage
{
    public enum State { FULL, DAMAGED, EMPTY };

    public float value;
	
	public Guage()
    {    
        value = 100f;   
    }
	
}
