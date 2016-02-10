using UnityEngine;
using System.Collections;

public interface ISpawnable
{
    Spawnable.Type spawnType { get; set; }
}

public class Spawnable
{
    public enum Type
    {
        UNDEFINED,
        BOSS_TURRET,
        HERO,
        PORTAL,
        PROJECTILE,
        ENEMY_TURRET,
        WEAPON_BLASTER,
        WEAPON_TURRET_CANNON
    };

    public static string GetResourceString(Type type)
    {
        switch (type)
        {
            case Type.BOSS_TURRET: return "Enemies/BossTurret";
            case Type.ENEMY_TURRET:
                return "Enemies/Turret";

            case Type.HERO: return "Hero";
            case Type.PORTAL: return "Portal";
            case Type.PROJECTILE: return "Projectile";
            case Type.WEAPON_BLASTER: return "Weapons/Blaster";
            case Type.WEAPON_TURRET_CANNON: return "Weapons/TurretCannon";

            default:
                throw new System.Exception("Undefined resource string for Spawnable.Type " + type.ToString());
        }
    }
}
