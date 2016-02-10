using UnityEngine;
using System.Collections;


public enum WeaponState {NONE, GUN }

abstract public class Weapon : MonoBehaviour, ISpawnable
{
    public float damage;
    public WeaponState state;
    public int weaponFireMode;
    public Spawnable.Type spawnType { get; set; }

    abstract public void Fire();
}
