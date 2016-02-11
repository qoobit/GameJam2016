using UnityEngine;
using System.Collections;

public enum GuageState { FULL, DAMAGED, EMPTY}
public class Guage : Object {
    public float value;
	// Use this for initialization
	public Guage() {
        
        value = 100f;
        
    }
	
}
