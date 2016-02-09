using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaLevel : Level
{
    private enum State { WARMUP, SPAWNBOSS, BOSSFIGHT, BOSSDEFEATED }

    private State state;
    private List<GameObject> turretList;
    private GameObject boss;
    private GameObject portalEntry;
    private GameObject portalExit;

    // Use this for initialization
    override protected void Start()
    {
        base.Start();

        //Get all existing turrets in scene and save to turretList
        turretList = new List<GameObject>();
        Turret[] turrets = GetComponentsInChildren<Turret>();
        foreach (Turret turret in turrets)
        {
            turretList.Add(turret.gameObject);
        }

        boss = null; //No boss yet
        state = State.WARMUP; //Set arena state to WARMUP

        //Get the exit portal and disable it for now
        portalEntry = GameObject.Find("Portal Entry");
        portalExit = GameObject.Find("Portal Exit");
        portalExit.SetActive(false);
    }

    // Update is called once per frame
    override protected void Update()
    {
        switch (state)
        {
            case State.WARMUP:
                updateWarmupState();
                break;

            case State.SPAWNBOSS:
                updateSpawnBossState();
                break;

            case State.BOSSFIGHT:
                updateBossFightState();
                break;

            case State.BOSSDEFEATED:
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
            state = State.SPAWNBOSS;
    }

    private void updateSpawnBossState()
    {
        //Spawn the boss
        if (boss == null)
            spawnBoss();
        
        //Check if we spawned correctly
        if (boss != null)
            state = State.BOSSFIGHT;
    }

    private void updateBossFightState()
    {
        if (boss == null || boss.GetComponent<BossTurret>().isAlive() == false)
            state = State.BOSSDEFEATED;
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

        EnemyWalk bossEnemyWalk = bossTurret.GetComponent<EnemyWalk>();
        GameObject waypointCollection = GameObject.Find("Waypoints");
        bossEnemyWalk.waypointCollection = waypointCollection;

        boss = bossTurret;
    }
}
