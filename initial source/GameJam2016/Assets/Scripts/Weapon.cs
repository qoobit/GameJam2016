using UnityEngine;
using System.Collections;


public enum WeaponState {NONE, GUN }
public class Weapon : Object {
    public float damage;
    public WeaponState state;

    public Weapon()
    {
        state = WeaponState.NONE;
    }
	// Use this for initialization
	
}
