using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public enum HeroState { IDLE, WALKING, JUMPING, DASHING, SHOOTING };
public enum DamageState { HEALTHY, STUNNED, DEAD };

public class Hero : Damageable {
    public float hp;
    public Vector3 hmdForward;
    public Vector3 facing;
    public DamageState damageState = DamageState.HEALTHY;
    public GameObject hmdRig;
    public GameObject Crosshair;

    Animator animator;

    bool dashing;
    bool jumping;
    bool shooting;
    bool charging;
   
    int jumpDash;
    
    public GameObject Focal;

    public AudioSource JumpAudio;
    public AudioSource ShotAudio;
    public AudioSource WalkAudio;
    public AudioSource DashAudio;
    public AudioSource DeathAudio;
    public AudioSource LockAudio;


    float walkAge = 0f;
    float walkDuration = 0.3f;
    float dashForce;
    float dashDuration = 0.5f;
    float dashAge = 0f;
    bool dashAllowed;
    bool airDashAllowed;
    bool jumpAllowed;
    float stunCooldown = 2f;
    float stunAge = 0f;
    int jumpHold = 0;
    int jumpLimit = 3;
    float jumpForce;
    
    
    Vector3 spawnPoint;
    
    float speed;
//    bool onFloor;

    Guage health;
    public Weapon weapon;

    bool lockToTarget;
    bool firstLock;

    GameObject lockedObject;
    
    public bool IsDashing(){return dashing;}
    public bool IsShooting() { return shooting; }
    public bool IsJumping() { return jumping; }
    public bool IsCharging() { return charging; }

    // Use this for initialization
    void Start () {
        jumpDash = 0;
        dashForce = 10000f;
        jumpForce = 12500f;

        dashing = charging = jumping = shooting = false;
        jumpHold = 0;
        lockToTarget = false;
        
        speed = 0.2f;

        //hp
        health = new Guage();
        spawnPoint = gameObject.transform.position;
        animator = GetComponent<Animator>();
        jumpAllowed = dashAllowed = false;
        firstLock = false;
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
            if(dashing)
            {
                //crash attack
                other.gameObject.GetComponent<Enemy>().Explode();
                dashAllowed = true;
            }
        }
        if ( other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if(dashing)
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
                //landed
                jumping = false;
                jumpDash = 0;
                jumpHold = 0;
                airDashAllowed = true;
                dashAllowed = true;
                jumpAllowed = true;
                GetComponent<Animator>().SetBool("jumping", false);
                
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

    void UpdateAnimator()
    {
        animator.SetBool("dashing", dashing);
        animator.SetBool("jumping", jumping);
        animator.SetBool("shooting", shooting);
        animator.SetFloat("health", health.value);
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(GameObject.Find("Focal").transform.localPosition+" "+ GameObject.Find("Focal").transform.position);
        
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


        if (dashing)
        {
            dashAge += Time.deltaTime;
            if (dashAge >= dashDuration)
            {
                stopDash();
                dashAllowed = true;
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }

       
        if (damageState != DamageState.DEAD)
        {
            applyInputsToCharacter();
        }

        UpdateAnimator();
        
        //Oculus look vector
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * 100f),Color.red);

        //facing vector
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + (facing * 100f), Color.green);
    }
    public void UpdateTransform()
    {
        
        float facingAngle = Vector3.Angle(Vector3.forward, facing);
        facingAngle *= Mathf.Sign(Vector3.Cross(Vector3.forward, facing).y);

        transform.localEulerAngles = new Vector3(0, facingAngle, 0);
    }
    public void RealignHMD()
    {
        Vector3 position = gameObject.transform.position - (facing * 7f);
        position += new Vector3(0, 7f, 0f);
         
        hmdRig.GetComponent<QoobitOVR>().Realign(position,Focal.transform);
    }
    void FocusTarget()
    {
        Vector3 directionToTarget = Vector3.zero;

        LayerMask layerMask = new LayerMask();
        layerMask = (1 << LayerMask.NameToLayer("Targets"));
        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));

        Crosshair.SetActive(false);

        if (hmdRig.GetComponent<QoobitOVR>().Discovered())
        {
            
        }

        if (lockToTarget && hmdRig.GetComponent<QoobitOVR>().FocusObject != null)
        {
            lockedObject = hmdRig.GetComponent<QoobitOVR>().FocusObject;
            if (!firstLock)
            {
                LockAudio.Play();
                firstLock = true;
            }
        }
        
        if (lockedObject != null)
        {

            Crosshair.SetActive(true);

            //face target
            directionToTarget = lockedObject.transform.position - transform.position;
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
    }

    override public void Hurt(float damage, GameObject attacker)
    {
        //Don't take damage if we are stunned
        if (stunAge > 0f) return;
    
        if (dashing && attacker.layer == LayerMask.NameToLayer("Enemy")) return;

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
        if (damageState == DamageState.DEAD) return;

        DeathAudio.Play();
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
        
    }



    private void applyInputsToCharacter()
    {
        float theta = Vector3.Angle(Vector3.forward, hmdForward);
        theta *= Mathf.Sign(Vector3.Cross(Vector3.forward, hmdForward).y);

        moveCharacter(theta);
        rotateCharacter(theta);

        //highlight object being looked at
        FocusTarget();
        
        doCharacterDash();
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
            
            if (walkAge <= 0f)
            {
                WalkAudio.Play();
            }
            walkAge += Time.deltaTime;
            
            float magnitude = movement.magnitude;
            if (magnitude > 1f) magnitude = 1f;
            walkDuration = (1f/magnitude) * 0.3f;
            if (walkAge >= walkDuration)
            {
                walkAge = 0f;
            }

            if (lockedObject == null)
            {
                facing = movementUnit;
            }
        }
        else
        {
            WalkAudio.Stop();
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

    private void doCharacterDash()
    {

        if (Input.GetButtonDown("B"))
        {

            if (!dashing && dashAllowed && jumpDash==0)
            {
                dashing = true;
                DashAudio.Play();

                if (!jumping) spawnPoint = gameObject.transform.position;
                else {
                    jumpDash++;
                    
                }

                dashAge = 0f;
                dashAllowed = false;
                
                Vector3 dashDirection = Vector3.zero;
                if (lockedObject != null)
                {
                    //move towards locked object
                    Vector3 directionToTarget = lockedObject.transform.position - gameObject.transform.position;
                    directionToTarget.Normalize();
                    dashDirection = directionToTarget;
                    
                }
                else {
                    //move towards facing direction
                    facing.Normalize();
                    dashDirection = facing;
                }

                GetComponent<Rigidbody>().AddForce(dashDirection * dashForce);
                GetComponent<Rigidbody>().useGravity = false;
            }
        }
        if (Input.GetButtonUp("B"))
        {
            if(jumpDash!=1||(jumpDash == 1 && airDashAllowed))
            {
                stopDash();
                airDashAllowed = false;
            }
            
        }
    }

    private void stopDash()
    {
        dashing = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = true;
        
    }

    private void stopJump()
    {
        jumping = false;
        GetComponent<Rigidbody>().isKinematic = true;
        
    }
    private void doCharacterJump()
    {
        if (Input.GetButtonDown("A"))
        {
            if(!jumping&&jumpAllowed)
            {
                JumpAudio.Play();
                gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                jumping = true;
                jumpAllowed = false;
                spawnPoint = gameObject.transform.position;
            }
        }
        if (Input.GetButtonUp("A"))
        {
            if(jumping) stopJump();
        }
    }

    private void doCharacterFire()
    {
        if (Input.GetButtonDown("X"))
        {
            if (weapon != null)
            {
                shooting = true;
                ShotAudio.Play();
                weapon.Fire();
            }
        }
        else if (Input.GetButtonUp("X"))
        {
            shooting = false;
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
            firstLock = false;
        }
    }
}
