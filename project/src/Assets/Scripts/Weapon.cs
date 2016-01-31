using UnityEngine;
using System.Collections;


public enum WeaponState {NONE, GUN }

abstract public class Weapon : MonoBehaviour
{
    public float damage;
    public WeaponState state;
    public int weaponFireMode;

    abstract public void Fire();
}
