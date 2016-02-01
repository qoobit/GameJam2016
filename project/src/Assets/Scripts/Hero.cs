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
    public GameObject Crosshair;

    public HeroState state = HeroState.IDLE;

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
    GameObject hmdLockedObject;
    
    // Use this for initialization
    void Start () {
        lockToTarget = false;
        
        speed = 0.2f;

        //hp
        health = new Guage();
        spawnPoint = gameObject.transform.position;

        dashAllowed = false;

        Physics.gravity = new Vector3(0, -50, 0);
        
        //Load a blaster as our weapon
        Object blasterObject = Resources.Load("Weapons/Blaster", typeof(GameObject));
        GameObject blasterWeapon = GameObject.Instantiate(blasterObject, gameObject.transform.position, this.transform.rotation) as GameObject;
        blasterWeapon.transform.parent = this.transform;
        blasterWeapon.transform.localPosition = new Vector3(blasterWeapon.transform.localPosition.x, blasterWeapon.transform.localPosition.y + 2f, blasterWeapon.transform.localPosition.z);
        weapon = blasterWeapon.GetComponent<Weapon>();



        //initialize hero with GameControl
        health.value = GameControl.control.health;

        
        
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
        if ( other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (state == HeroState.DASHING)
            {
                //crash attack
                other.gameObject.GetComponent<Damageable>().Hurt(100f, this.gameObject);
                dashAllowed = true;
            }
        }




    }
    void OnCollisionEnter(Collision col)
    {
        

        if (GameControl.control.level != null) {
            if (col.gameObject.tag == "Platform")
            {
                state = HeroState.IDLE;
                onFloor = true;
                dashAllowed = true;
                GetComponent<Animator>().SetBool("jumping", false);
                GetComponent<Animator>().SetBool("dashing", false);
            }

        }
            
        
    }

    void SetSkinnedMeshRendererEnabled(GameObject go, bool enabled)
    {
        if (go.GetComponent<SkinnedMeshRenderer>() != null)
        {
            go.GetComponent<SkinnedMeshRenderer>().enabled = enabled;
        }
        foreach (Transform child in go.transform)
        {
            SetSkinnedMeshRendererEnabled(child.gameObject, enabled);
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

    void SetSkinnedMeshRendererColor(GameObject go, Color c)
    {
        if (go.GetComponent<SkinnedMeshRenderer>() != null)
        {

            go.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Mode", 2);
            go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //go.GetComponent<SkinnedMeshRenderer>().material.SetInt("_ZWrite", 0);
            //go.GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
            //go.GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_ALPHABLEND_ON");
            go.GetComponent<SkinnedMeshRenderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //go.GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3000;

            go.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", c);
        }
        foreach (Transform child in go.transform)
        {
            SetSkinnedMeshRendererColor(child.gameObject, c);
        }
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(GameObject.Find("Focal").transform.localPosition+" "+ GameObject.Find("Focal").transform.position);
        //Debug.Log(dashAllowed + " " + state + " " + onFloor);
        ((Blaster)weapon).bulletReloadTime = 0.01f;
        ((Blaster)weapon).clipReloadTime = 0f;
        weapon.weaponFireMode = (int)BlasterMode.STRAIGHT;

        //update viewers
        hp = health.value;

        hmdForward = Camera.main.transform.forward;
        hmdForward.y = 0f;
        hmdForward.Normalize();
        

        //blink stunned hero
        if (damageState == DamageState.STUNNED)
        {
            SetSkinnedMeshRendererEnabled(gameObject,!gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled);
            stunAge -= Time.deltaTime;
            if (stunAge <= 0f)
            {
                damageState = DamageState.HEALTHY;
                SetSkinnedMeshRendererEnabled(gameObject, true);
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
                    dashAllowed = true;
                    GetComponent<Animator>().SetBool("dashing", false);
                }
                break;
            
            default:
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                break;
        }

        applyInputsToCharacter();        

        //Debug Lines



        //Oculus look vector
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * 100f),Color.red);

        //facing vector
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (facing * 100f), Color.green);
    }
    public void UpdateTransform()
    {
        facing.Normalize();
        float facingAngle = Vector3.Angle(Vector3.forward, facing);
        facingAngle *= Mathf.Sign(Vector3.Cross(Vector3.forward, facing).y);
        //Debug.Log(Vector3.forward + " " + direction + " " + "ANGLE:" + lookAngle);
        gameObject.transform.localEulerAngles = new Vector3(0, facingAngle, 0);
    }
    public void RealignHMD()
    {
        //gameObject.transform.position = spawnPoint;
        //Debug.Log(gameObject.transform.position+" "+facing+" "+ GameObject.Find("Focal").transform.position);
        cameraRig.transform.position = gameObject.transform.position - (facing * 10f);
        cameraRig.transform.position += new Vector3(0, 20f, 0f);
        cameraRig.transform.LookAt(GameObject.Find("Focal").transform);
    }
    Vector3 HighlightTarget()
    {
        if (GameControl.control.level == null) return Vector3.zero;
        RaycastHit hit;
        Material whiteMat = Resources.Load("Materials/White", typeof(Material)) as Material;
        Material greenMat = Resources.Load("Materials/Green", typeof(Material)) as Material;
        Vector3 directionToTarget = Vector3.zero;

        hmdLockedObject = null;
        //reset materials to white

        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");


        for (int i = 0; i < targets.Length; i++)
        {
            

            if (targets[i].GetComponent<MeshRenderer>() != null)
            {
                targets[i].GetComponent<MeshRenderer>().material = whiteMat;
            }
            SetSkinnedMeshRendererColor(targets[i], new Color(1f, 1f, 1f,1f));
                
        }

        //highlight
        LayerMask layerMask = new LayerMask();
        layerMask = (1 << LayerMask.NameToLayer("Targets"));
        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));

        Crosshair.SetActive(false);
        //Debug.Log(Camera.main.gameObject.transform.GetChild(0).gameObject.name);
        Camera.main.gameObject.transform.GetChild(0).gameObject.SetActive(false);


        if (Physics.SphereCast(Camera.main.transform.position, 5f, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask ))
        {
            if (hit.collider.gameObject.GetComponent<MeshRenderer>() != null)
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().material = greenMat;
            }

            SetSkinnedMeshRendererColor(hit.collider.gameObject, new Color(0, 255f/255f, 86f/255f,0f));
            
            Camera.main.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            hmdLockedObject = hit.collider.gameObject;

            Vector3 targetCenter = hmdLockedObject.transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center);
            Vector3 directionToCenter = targetCenter - Camera.main.transform.position;
            directionToCenter.Normalize();
            Camera.main.gameObject.transform.GetChild(0).gameObject.transform.position = targetCenter - (directionToCenter * 3f);

            if (lockToTarget&&lockedObject==null)
            {
                lockedObject = hmdLockedObject;
            }
            
        }


        if (lockedObject != null)
        {
            Crosshair.SetActive(true);

            //face target
            directionToTarget = lockedObject.transform.position - gameObject.transform.position;
            directionToTarget.Normalize();
            facing = directionToTarget;
            float angleToTarget = Vector3.Angle(Vector3.forward, directionToTarget);
            angleToTarget *= Mathf.Sign(Vector3.Cross(Vector3.forward, directionToTarget).y);

            gameObject.transform.localEulerAngles = new Vector3(0, angleToTarget, 0);

            Vector3 heroCenter = gameObject.transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center);
            Vector3 targetCenter = lockedObject.transform.TransformPoint(gameObject.GetComponent<CapsuleCollider>().center);
            Vector3 directionToCenter = targetCenter - heroCenter;

            directionToCenter.Normalize();
            Crosshair.transform.position = targetCenter - (directionToCenter * 3f);
        }

        return directionToTarget;
    }

    override public void Hurt(float damage, GameObject attacker)
    {
        //Don't take damage if we are stunned
        if (stunAge > 0f) return;

        if ((state == HeroState.DASHING || state == HeroState.AIR_DASHING) && attacker.layer == LayerMask.NameToLayer("Enemy")) return;

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
        /*
        if (GameControl.control.lives == 0)
        {
            //game over

        }
        else
        {
        */
        GetComponent<Animator>().SetFloat("health", health.value);
            //Respawn();
        /*
        }
        */
    }

    public void Respawn()
    {
        //goto spawn point
        gameObject.transform.position = spawnPoint;
        health.value = 100f;
        damageState = DamageState.STUNNED;
        stunAge = stunCooldown;
        RealignHMD();
        GetComponent<Animator>().SetFloat("health", health.value);
    }



    private void applyInputsToCharacter()
    {
        float theta = Vector3.Angle(Vector3.forward, hmdForward);
        theta *= Mathf.Sign(Vector3.Cross(Vector3.forward, hmdForward).y);

        moveCharacter(theta);
        rotateCharacter(theta);

        //highlight object being looked at
        Vector3 directionToTarget = HighlightTarget();

        doCharacterDash(directionToTarget);
        doCharacterJump();
        doCharacterFire();
        doRealignHMD();
        doLockToTarget();
    }

    private void moveCharacter(float theta)
    {
        //controller movement
        Vector3 movement = new Vector3(-Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY"));
        Vector3 movementUnit = movement.normalized;
        movementUnit = Quaternion.Euler(0, 180, 0) * movementUnit;

        if (movement.magnitude != 0f)
        {
            movementUnit = Quaternion.Euler(0, theta, 0) * movementUnit;
            gameObject.transform.position += movementUnit * speed * Mathf.Abs(movement.magnitude);

            if(state!=HeroState.DASHING) state = HeroState.WALKING;
            if (lockedObject == null)
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

        GetComponent<Animator>().SetFloat("speed", movement.magnitude);
    }

    private void rotateCharacter(float theta)
    {
        //controller rotation
        Vector3 direction = new Vector3(-Input.GetAxisRaw("RightJoystickX"), 0, Input.GetAxisRaw("RightJoystickY"));
        Vector3 directionUnit = direction.normalized;
        directionUnit = Quaternion.Euler(0, 180, 0) * directionUnit;

        if (direction.magnitude != 0f)
        {
            facing = Quaternion.Euler(0, theta, 0) * directionUnit;
            if (lockedObject==null)
            {
                facing = direction;
                //facing = direction;
                
            }
            else
            {
                Vector3 directionToTarget = lockedObject.transform.position - gameObject.transform.position;
                directionToTarget.Normalize();
                facing = directionToTarget;
                
            }
        }

        UpdateTransform();
    }

    private void doCharacterDash(Vector3 directionToTarget)
    {
        if (Input.GetButtonDown("B"))
        {
            
            if (state != HeroState.DASHING && dashAllowed)
            {
                GetComponent<Animator>().SetBool("dashing", true);
                GetComponent<Animator>().SetBool("dashing", false);
                if (onFloor) spawnPoint = gameObject.transform.position;
                if (lockedObject != null)
                {
                    directionToTarget = lockedObject.transform.position - gameObject.transform.position;
                    directionToTarget.Normalize();
                    gameObject.GetComponent<Rigidbody>().AddForce(directionToTarget * 20000f);
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    state = HeroState.DASHING;
                    dashAge = 0f;
                    //onFloor = false;
                    dashAllowed = false;
                }
                else {
                    facing.Normalize();
                    gameObject.GetComponent<Rigidbody>().AddForce(facing * 10000f);
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    state = HeroState.DASHING;
                    dashAge = 0f;
                    //onFloor = false;

                    dashAllowed = false;
                }
            }
        }
    }

    private void doCharacterJump()
    {
        if (Input.GetButtonDown("A"))
        {
            if (onFloor)
            {
                GetComponent<Animator>().SetBool("jumping", true);
                GetComponent<Animator>().SetBool("dashing", false);
                gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000f);
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                state = HeroState.JUMPING;
                onFloor = false;
                spawnPoint = gameObject.transform.position;

            }
        }
    }

    private void doCharacterFire()
    {
        if (Input.GetButtonDown("X"))
        {
            if (weapon != null)
            {
                GetComponent<Animator>().SetBool("shooting", true);
                weapon.Fire();
            }
        }
        else if (Input.GetButtonUp("X"))
        {
            GetComponent<Animator>().SetBool("shooting", false);
        }
    }

    private void doRealignHMD()
    {
        if (Input.GetButtonDown("RB"))
        {
            RealignHMD();
        }
        
    }

    private void doLockToTarget()
    {
        if (Input.GetAxisRaw("RT") < 0f)
        {
            //lock
            lockToTarget = true;
        }
        else
        {
            lockToTarget = false;
            lockedObject = null;
        }
    }
}
