using UnityEngine;
using System.Collections;

abstract public class Damageable : MonoBehaviour
{
    abstract public void Hurt(float damage, GameObject attacker);
    abstract public void Die();
}
