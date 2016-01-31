using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HeroState { IDLE, WALKING, JUMPING, DASHING, AIR_DASHING, SHOOTING };
public enum DamageState { HEALTHY, STUNNED, DEAD };

public class Hero : Damageable {
    public float hp;
    public Vector3 hmdForward;
    public Vector3 facing;
    public DamageState damageState = DamageState.HEALTHY;
    public GameObject cameraRig;

    HeroState state = HeroState.IDLE;

    float dashDuration = 0.5f;
    float dashAge = 0f;
    public bool dashAllowed;
    float stunCooldown = 2f;
    float stunAge = 0f;
    
    Vector3 spawnPoint;
    
    float speed;
    bool onFloor;

    Guage health;
    public Weapon weapon;

    bool lockToTarget;
    GameObject lockedObject;


    
	// Use this for initialization
	void Start () {
        lockToTarget = false;
        facing = new Vector3(0, 0, 1);
        speed = 0.2f;
        //hp
        health = new Guage();
        spawnPoint = gameObject.transform.position;
        dashAllowed = false;

        Physics.gravity = new Vector3(0, -50, 0);
        
        //Load a blaster as our weapon
        Object blasterObject = Resources.Load("Weapons/Blaster", typeof(GameObject));
        GameObject blasterWeapon = GameObject.Instantiate(blasterObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        blasterWeapon.transform.parent = this.transform;
        weapon = blasterWeapon.GetComponent<Weapon>();
    }
    

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Targets"))
        {
            if (state == HeroState.DASHING)
            {
                //crash attack
                other.gameObject.GetComponent<Enemy>().Explode();
                dashAllowed = true;
            }
        }


    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Terrain")
        {
            //Debug.Log("on terrain");
            state = HeroState.IDLE;
            onFloor = true;
            dashAllowed = true;
        }
        
        else
        {
            for (int i = 0; i < GameControl.control.level.GetComponent<Level>().platforms.Count; i++)
            {
                //Debug.Log(col.gameObject.name);
                
                if(col.gameObject.name == GameControl.control.level.GetComponent<Level>().platforms[i].name)
                {
                    state = HeroState.IDLE;
                    onFloor = true;
                    dashAllowed = true;
                    break;
                }
            
            }
        }
    }


    void SetMeshRendererEnabled(GameObject go,bool enabled)
    {
        if (go.GetComponent<MeshRenderer>() != null)
        {
            go.GetComponent<MeshRenderer>().enabled = enabled;
        }
        foreach(Transform child in go.transform)
        {
            SetMeshRendererEnabled(child.gameObject, enabled);
        }
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(state);

        //update viewers
        hp = health.value;

        hmdForward = Camera.main.transform.forward;
        hmdForward.y = 0f;
        hmdForward.Normalize();
        

        //blink stunned hero
        if (damageState == DamageState.STUNNED)
        {
            SetMeshRendererEnabled(gameObject,!GetComponent<MeshRenderer>().enabled);
            stunAge -= Time.deltaTime;
            if (stunAge <= 0f)
            {
                damageState = DamageState.HEALTHY;
                SetMeshRendererEnabled(gameObject, true);
            }
        }


        //turn physics back on
        if (gameObject.GetComponent<Rigidbody>().isKinematic) {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }


        switch (state)
        {
            case HeroState.DASHING:

                dashAge += Time.deltaTime;
                if (dashAge >= dashDuration)
                {
                    //state = HeroState.JUMPING;
                    state = HeroState.IDLE;
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                }
                break;
            
            default:
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                break;
        }

        //controller movement
        Vector3 movement = new Vector3(-Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY"));
        Vector3 movementUnit = movement.normalized;
        movementUnit = Quaternion.Euler(0, 180, 0) * movementUnit;

        float theta = Vector3.Angle(Vector3.forward, hmdForward);
        theta *= Mathf.Sign(Vector3.Cross(Vector3.forward, hmdForward).y);

        if (state != HeroState.DASHING)
        {
            if (movement.magnitude != 0f)
            {
                movementUnit = Quaternion.Euler(0, theta, 0) * movementUnit;
                gameObject.transform.position += movementUnit * speed * Mathf.Abs(movement.magnitude);
                
                state = HeroState.WALKING;
                if (!lockToTarget)
                {
                    facing = movementUnit;
                }
            }
            else
            {
                if (state == HeroState.WALKING)
                {
                    state = HeroState.IDLE;
                }
                
            }
        }
        

        //controller rotation
        Vector3 direction = new Vector3(-Input.GetAxisRaw("RightJoystickX"), 0, Input.GetAxisRaw("RightJoystickY"));
        Vector3 directionUnit = direction.normalized;
        directionUnit = Quaternion.Euler(0, 180, 0) * directionUnit;

        if (direction.magnitude != 0f)
        {
            facing = Quaternion.Euler(0, theta, 0) * directionUnit;
            if (!lockToTarget)
            {
                facing = direction;
                //facing = direction;
            }
        }
        
        facing.Normalize();
        float facingAngle = Vector3.Angle(Vector3.forward, facing);
        facingAngle *= Mathf.Sign(Vector3.Cross(Vector3.forward, facing).y);
        //Debug.Log(Vector3.forward + " " + direction + " " + "ANGLE:" + lookAngle);
        gameObject.transform.localEulerAngles = new Vector3(0, facingAngle, 0);

        
        //highlight object being looked at
        RaycastHit hit;
        Material whiteMat = Resources.Load("Materials/White", typeof(Material)) as Material;
        Material greenMat = Resources.Load("Materials/Green", typeof(Material)) as Material;
        Vector3 directionToTarget = Vector3.zero;

        lockedObject = null;
        //reset materials to white
        for (int i = 0; i < GameControl.control.level.GetComponent<Level>().targets.Count; i++)
        {
            if (GameControl.control.level.GetComponent<Level>().targets[i] != null)
            {
                if(GameControl.control.level.GetComponent<Level>().targets[i].GetComponent<MeshRenderer>()!=null) GameControl.control.level.GetComponent<Level>().targets[i].GetComponent<MeshRenderer>().material = whiteMat;
            }
        }

        //highlight
        for (int i = 0; i < GameControl.control.level.GetComponent<Level>().targets.Count; i++)
        {

            if (Physics.SphereCast(Camera.main.transform.position, 3f, Camera.main.transform.forward, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Targets")))
            {
                if (hit.collider.gameObject.GetComponent<MeshRenderer>() != null)  hit.collider.gameObject.GetComponent<MeshRenderer>().material = greenMat ;

                lockedObject = hit.collider.gameObject;

                if (lockToTarget) { 
                    
                    directionToTarget = hit.collider.gameObject.transform.position - gameObject.transform.position;
                    facing = directionToTarget;
                    float angleToTarget = Vector3.Angle(Vector3.forward, directionToTarget);
                    angleToTarget *= Mathf.Sign(Vector3.Cross(Vector3.forward, directionToTarget).y);
                    
                    gameObject.transform.localEulerAngles = new Vector3(0, angleToTarget, 0);
                }
                break;
            }

        }


        
        //controller buttons
        if (Input.GetButtonDown("A"))
        {
            if (state!=HeroState.DASHING)
            {
                
                if(onFloor)
                {
                    gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000f);
                    gameObject.GetComponent<Rigidbody>().useGravity = true;
                    state = HeroState.JUMPING;
                    onFloor = false;
                    dashAllowed = false;
                    Debug.Log("LEAVING");
                    
                }
                
            }
        }
        else if (Input.GetButtonDown("B"))
        {
            if (state != HeroState.DASHING)
            {
                if ((lockToTarget && lockedObject != null))
                {
                    directionToTarget = lockedObject.transform.position - gameObject.transform.position;
                    Debug.Log(lockedObject.transform.position + " " + gameObject.transform.position + " " + directionToTarget);
                    directionToTarget.Normalize();
                    gameObject.GetComponent<Rigidbody>().AddForce(directionToTarget * 20000f);
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    state = HeroState.DASHING;
                    dashAge = 0f;
                    onFloor = false;
                    dashAllowed = false;
                }
                else {
                    facing.Normalize();
                    gameObject.GetComponent<Rigidbody>().AddForce(facing * 20000f);
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    state = HeroState.DASHING;
                    dashAge = 0f;
                    onFloor = false;
                    dashAllowed = false;
                }


            }
        }

        if (Input.GetButtonDown("X"))
        {
            if (weapon != null)
                weapon.Fire();
        }

        if (Input.GetButtonDown("RB"))
        {
            //gameObject.transform.position = spawnPoint;
            cameraRig.transform.position = gameObject.transform.position - (facing * 10f);
            cameraRig.transform.position+= new Vector3(0,10f,0f);
            cameraRig.transform.LookAt(GameObject.Find("Focal").transform);
            
        }

            if (Input.GetAxisRaw("RT") < 0f)
        {
            //lock
            lockToTarget = true;
        }
        else
        {
            lockToTarget = false;
        }
        

        //Debug Lines



        //Oculus look vector
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * 100f),Color.red);

        //facing vector
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (facing * 100f), Color.green);
    }

    override public void Hurt(float damage, GameObject attacker)
    {
        //Don't take damage if we are stunned
        if (stunAge > 0f) return;

        health.value -= damage;
        if (health.value <= 0)
        {
            this.Die();
        }
        else
        {
            damageState = DamageState.STUNNED;
            stunAge = stunCooldown;
        }
    }

    override public void Die()
    {
        damageState = DamageState.DEAD;
        GameControl.control.lives--;
        if (GameControl.control.lives == 0)
        {
            //game over

        }
        else
        {
            //goto spawn point
            gameObject.transform.position = spawnPoint;
            health.value = 100f;
            damageState = DamageState.STUNNED;
            stunAge = stunCooldown;
        }
    }
}
