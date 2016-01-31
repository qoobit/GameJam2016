using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{

    public GameObject CanvasCamera;
    public GameObject Hero;

    public List<GameObject> platforms = new List<GameObject>();
    public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> targets = new List<GameObject>();

    public List<GameObject> portals = new List<GameObject>();


    void AddTarget(GameObject t)
    {
        if (t != null) targets.Add(t);
    }
    void AddPlatform(GameObject platform)
    {
        if (platform != null) platforms.Add(platform);
    }

    // Use this for initialization
    void Start()
    {
        AddPlatform(GameObject.Find("platform"));
        AddPlatform(GameObject.Find("Plane"));
        AddPlatform(GameObject.Find("platform (1)"));
        AddPlatform(GameObject.Find("platform (2)"));
        AddPlatform(GameObject.Find("platform (3)"));
        AddPlatform(GameObject.Find("platform (4)"));
        AddPlatform(GameObject.Find("level_one_piece01_start"));
        AddPlatform(GameObject.Find("level_one_piece02_mid"));
        AddPlatform(GameObject.Find("level_one_piece03_end"));
        AddPlatform(GameObject.Find("Nudger A"));
        AddPlatform(GameObject.Find("Nudger B"));
        AddPlatform(GameObject.Find("Nudger C"));

        AddTarget(GameObject.Find("Enemy"));
        AddTarget(GameObject.Find("Pit"));
        AddTarget(GameObject.Find("Target"));
        AddTarget(GameObject.Find("Target (1)"));
        AddTarget(GameObject.Find("Target (2)"));
        AddTarget(GameObject.Find("Target (3)"));
        AddTarget(GameObject.Find("Target (4)"));
        AddTarget(GameObject.Find("Target (5)"));
        AddTarget(GameObject.Find("Target (6)"));
        AddTarget(GameObject.Find("Target (7)"));
        AddTarget(GameObject.Find("Target (8)"));
        AddTarget(GameObject.Find("Target (9)"));
        AddTarget(GameObject.Find("Target (10)"));

        AddTarget(GameObject.Find("Turret"));
        AddTarget(GameObject.Find("Turret (1)"));
        AddTarget(GameObject.Find("Turret (2)"));


        CanvasCamera.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
