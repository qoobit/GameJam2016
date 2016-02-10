using UnityEngine;
using System.Collections;

public interface IDamageable
{
    void Hurt(float damage, GameObject attacker);
    void Die();
}
