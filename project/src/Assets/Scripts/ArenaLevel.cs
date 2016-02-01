using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaLevel : MonoBehaviour
{
    private enum ArenaState { WARMUP, SPAWNBOSS, BOSSFIGHT, BOSSDEFEATED }

    private ArenaState arenaState;
    private List<GameObject> turretList;
    private GameObject boss;
    private GameObject portalEntry;
    private GameObject portalExit;

    // Use this for initialization
    void Start()
    {
        

        //Get all existing turrets in scene and save to turretList
        turretList = new List<GameObject>();
        Turret[] turrets = GetComponentsInChildren<Turret>();
        foreach (Turret turret in turrets)
        {
            turretList.Add(turret.gameObject);
        }

        boss = null; //No boss yet
        arenaState = ArenaState.WARMUP; //Set arena state to WARMUP

        //Get the exit portal and disable it for now
        portalEntry = GameObject.Find("Portal Entry");
        portalExit = GameObject.Find("Portal Exit");
        portalExit.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        GameControl.control.level = this.gameObject;

        switch (arenaState)
        {
            case ArenaState.WARMUP:
                updateWarmupState();
                break;

            case ArenaState.SPAWNBOSS:
                updateSpawnBossState();
                break;

            case ArenaState.BOSSFIGHT:
                updateBossFightState();
                break;

            case ArenaState.BOSSDEFEATED:
                updateBossDefeatedState();
                break;
        }
    }

    private void updateWarmupState()
    {
        //If all warm up turrets are dead, spawn the boss
        List<GameObject> newTurretList = new List<GameObject>();
        foreach (GameObject turret in turretList)
        {
            if (turret != null)
                newTurretList.Add(turret);
        }

        turretList = newTurretList;

        if (turretList.Count == 0)
            arenaState = ArenaState.SPAWNBOSS;
    }

    private void updateSpawnBossState()
    {
        if (boss == null)
            spawnBoss();
        else if (boss.transform.position.y <= 0.75f)
        {
            //Check if boss has reached the ground (falling from sky)
            //Enable the navmesh and begin patrolling.
            NavMeshAgent bossAgent = boss.GetComponent<NavMeshAgent>();
            bossAgent.enabled = true;

            arenaState = ArenaState.BOSSFIGHT;
        }
    }

    private void updateBossFightState()
    {
        if (boss==null||boss.GetComponent<BossTurret>().isAlive() == false)
            arenaState = ArenaState.BOSSDEFEATED;
    }

    private void updateBossDefeatedState()
    {
        //Boss is defeated, activate the exit portal
        portalExit.SetActive(true);

        // Explode all minions
        foreach (GameObject turret in turretList)
        {
            if (turret != null)
            {
                turret.GetComponent<Turret>().Explode();
            }
        }
    }

    private void spawnBoss()
    {
        GameObject bossSpawn = GameObject.Find("BossSpawn");

        //Spawn the boss close to NavMesh at the beginning
        Object bossTurretObject = Resources.Load("Enemies/BossTurret", typeof(GameObject));
        GameObject bossTurret = GameObject.Instantiate(bossTurretObject, Vector3.zero, Quaternion.identity) as GameObject;
        bossTurret.name = "Boss";
        bossTurret.layer = LayerMask.NameToLayer("Enemy");
        NavMeshAgent bossAgent = bossTurret.GetComponent<NavMeshAgent>();
        bossAgent.enabled = false;
        bossTurret.transform.position = bossSpawn.transform.position; //Move boss to bossSpawn after navMeshAgent is created
        bossTurret.transform.rotation = bossSpawn.transform.rotation;

        EnemyBase bossEnemyBase = bossTurret.GetComponent<EnemyBase>();
        GameObject waypointCollection = GameObject.Find("Waypoints");
        bossEnemyBase.WaypointCollection = waypointCollection;

        boss = bossTurret;
    }
}
