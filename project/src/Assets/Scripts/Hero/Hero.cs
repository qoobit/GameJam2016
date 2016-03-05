using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Hero : StateEntity, IDamageable
{

    
    public enum DamageState { HEALTHY, STUNNED, DEAD };

    
    #region Status
    public Vector3 facing;
    public float hp;
    public Vector3 hmdForward;
    public DamageState damageState = DamageState.HEALTHY;

    public Guage health;
    public Vector3 velocity = Vector3.zero;

    bool lockToTarget;
    bool firstLock;

    public GameObject lockedObject;
    #endregion
    #region Public Game Objects
    public GameObject hmdRig;
    public GameObject weapon;
    public GameObject Crosshair;
    public GameObject wall;
    public GameObject platform;
    public GameObject GrabbingGameObject;
    public GameObject LiftingGameObject;
    public GameObject LiftingReference;
    public GameObject TouchingGameObject;

    public GameObject Root;             //root game object of meshes
    public GameObject Focal;


    public float jumpForce = 20f;
    public float dashForce;

    Vector3 spawnPoint;
    Vector3 wallNormal;

    #endregion
    #region Gizmos
    public Color GizmosFacingColor = Color.green;
    public float GizmosFacingLength = 5f;
    #endregion
    #region State Flags

    bool dashing;
    bool jumping;
    bool shooting;
    bool charging;
    bool scaling;
    bool standing;
    int jumpDash;
    bool grabbing;
    bool lifting;

    #endregion


    #region Limits
    public float gravity = -200f;
    public float terminalVelocity = 15f;
    float walkAge = 0f;
    float walkDuration = 0.3f;
    
    float dashDuration = 0.5f;
    float dashAge = 0f;
    bool dashAllowed;
    bool airDashAllowed;
    bool jumpAllowed;
    float stunCooldown = 2f;
    float stunAge = 0f;
    int jumpHold = 0;
    int jumpLimit = 3;
    float touchingAngle = 45f;

    float jumpAge = 0f;
    float jumpDuration = 0.25f;
    
    float speed;
    float speedScale;

    #endregion

    private HeroAudio heroAudio;

    public bool IsStanding { get { return standing; } }
    public bool IsDashing { get { return dashing; } }
    public bool IsShooting { get { return shooting; } }
    public bool IsJumping { get { return jumping; } }
    public bool IsGrabbing { get { return grabbing; } }
    public bool IsLifting { get { return lifting; } }
    public bool IsCharging { get { return charging; } }
    public bool IsScaling { get { return scaling; } }
    public float Speed { get { return speed; } }
    public float Health
    {
        get
        {
            return (health == null ? 0f : health.value);
        }
        set
        {
            if (health == null) return;
            health.value = value;
        }
    }
    
    override protected void Start()
    {
        base.Start();

        dashing = charging = jumping = shooting = scaling = standing = false;
        lockToTarget = false;

        jumpDash = 0;
        dashForce = 20f;
        gravity = -200f;
        jumpHold = 0;
        
        
        speedScale = 20f;

        //hp
        health = new Guage();
        
        spawnPoint = gameObject.transform.position;
        jumpAllowed = dashAllowed = false;
        firstLock = false;

        //Load a blaster as our weapon
        weapon = GameControl.Spawn(Spawnable.Type.WEAPON_FIST_BLASTER, this.transform.position, this.transform.rotation);
        weapon.name = "Fist Blaster";
        weapon.transform.parent = this.transform;
        weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x, weapon.transform.localPosition.y + 2f, weapon.transform.localPosition.z);
        
        //initialize hero with GameControl
        health.value = GameControl.control.health;

        //References
        heroAudio = GetComponent<HeroAudio>();

        hmdRig.GetComponent<QoobitOVR>().FollowObject = this.gameObject;
    }

    #region Collision Detection
    void OnTriggerExit(Collider other)
    {
        if (GameControl.control.level == null) return;
        //Debug.Log("OUT:" + other.gameObject.name);
        
        TouchingGameObject = null;
        
    }
    void OnTriggerStay(Collider other)
    {
        if (GameControl.control.level == null) return;
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
                other.gameObject.GetComponent<IDamageable>().Hurt(100f, this.gameObject);
                dashAllowed = true;
            }
        }
        
        //Debug.Log("IN:" + other.gameObject.name);
        

        //get angle between hero and object
        Vector3 directionToObject = other.gameObject.transform.position - transform.position;
        directionToObject.y = 0f;//projected to floor
        float angle = Vector3.Angle(directionToObject, facing);
        if (angle <= touchingAngle)
        {
            TouchingGameObject = other.gameObject;
        }
        else {
            TouchingGameObject = null;
        }
        

    }
    void OnCollisionStay(Collision col)
    {
        if (GameControl.control.level == null) return;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (GameControl.control.level == null) return;
    }
    void OnCollisionExit(Collision collision)
    {
        if (GameControl.control.level == null) return;
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (GameControl.control.level == null) return;
        
        if (col.gameObject.tag == "Platform")
        {
                
            platform = col.gameObject;

            //landed
            //jumping = false;
            scaling = false;
            standing = true;
            jumpDash = 0;
            jumpHold = 0;
                
            airDashAllowed = true;
            dashAllowed = true;
                
        }
        else if (col.gameObject.tag == "Wall")
        {
            wall = col.gameObject;
            if(!standing) scaling = true;
            dashAllowed = true;
            jumpDash = 0;
            jumpAllowed = true;
            wallNormal = col.normal;
        }
        else
        {
            wall = null;
            jumpAllowed = false;
            scaling = false;
            
        }
    }
    #endregion


    #region Helper Functions
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
    #endregion

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        GetComponent<CharacterController>().Move(velocity * Time.deltaTime);

        //Debug.Log(GameObject.Find("Focal").transform.localPosition+" "+ GameObject.Find("Focal").transform.position);

        ((FistBlaster)weapon.GetComponent<Weapon>()).bulletReloadTime = 0.01f;
        ((FistBlaster)weapon.GetComponent<Weapon>()).clipReloadTime = 0f;
        weapon.GetComponent<Weapon>().weaponFireMode = (int)BlasterMode.STRAIGHT;

        //update viewers

        hmdForward = Camera.main.transform.forward;
        hmdForward.y = 0f;
        hmdForward.Normalize();

        


        //Debug.Log(GetComponent<CharacterController>().isGrounded);

        if (platform != null)
        {
            jumpAllowed = true;
        }
        if (GetComponent<CharacterController>().isGrounded)
        {
            dashAllowed = true;
            velocity.y = 0;
        }
        else
        {
            platform = null;
            //jumpAllowed = false;
            if (!dashing)
            {
                if (scaling)
                {
                    //apply gravity
                    velocity.y += gravity *0.5f* Time.deltaTime; 
                }
                else {
                    //apply gravity
                    velocity.y += gravity * Time.deltaTime;
                }
            }
            else
            {
                velocity.y = 0f;
            }
        }

        if (velocity.y < -terminalVelocity) velocity.y = -terminalVelocity;
        if (velocity.y > terminalVelocity) velocity.y = terminalVelocity;



        //blink stunned hero
        if (damageState == DamageState.STUNNED)
        {
            SetSkinnedMeshRendererEnabled(gameObject,!Root.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled);
            stunAge -= Time.deltaTime;
            if (stunAge <= 0f)
            {
                damageState = DamageState.HEALTHY;
                SetSkinnedMeshRendererEnabled(gameObject, true);
            }
        }

        if (damageState != DamageState.DEAD)
        {
            applyInputsToCharacter();
        }       
    }

    public void UpdateTransform()
    {
        
        float facingAngle = Vector3.Angle(Vector3.forward, facing);
        facingAngle *= Mathf.Sign(Vector3.Cross(Vector3.forward, facing).y);

        transform.localEulerAngles = new Vector3(0, facingAngle, 0);
    }


    //realign HMD to behind the hero
    
    
    void FocusTarget()
    {
        Vector3 directionToTarget = Vector3.zero;
        /*
        LayerMask layerMask = new LayerMask();
        layerMask = (1 << LayerMask.NameToLayer("Targets"));
        layerMask |= (1 << LayerMask.NameToLayer("Enemy"));
        */
        Crosshair.SetActive(false);

        if (hmdRig.GetComponent<QoobitOVR>().Discovered())
        {
            
        }

        if (lockToTarget && hmdRig.GetComponent<QoobitOVR>().FocusObject != null)
        {
            lockedObject = hmdRig.GetComponent<QoobitOVR>().FocusObject;
            if (!firstLock)
            {
                heroAudio.Play(HeroAudio.Clip.LOCK);
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

            Vector3 heroCenter = gameObject.transform.TransformPoint(gameObject.GetComponent<CharacterController>().center);
            Vector3 targetCenter = lockedObject.transform.TransformPoint(gameObject.GetComponent<CharacterController>().center);
            Vector3 directionToCenter = targetCenter - heroCenter;

            directionToCenter.Normalize();
            Crosshair.transform.position = targetCenter - (directionToCenter * 3f);
        }
    }

    public void Hurt(float damage, GameObject attacker)
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

    public void Die()
    {
        if (damageState == DamageState.DEAD) return;
        velocity.x = velocity.z = 0f;
        heroAudio.Play(HeroAudio.Clip.DEATH);
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
        hmdRig.GetComponent<QoobitOVR>().Realign(gameObject.transform.position,facing);

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
        doCharacterMainAction();
        doRealignHMD();
        doLockToTarget();

    }

    private void moveCharacter(float theta)
    {
        

        //controller movement
        Vector3 movement = new Vector3(-InputDeviceController.control.HorizontalAxis, 0, InputDeviceController.control.VerticalAxis);
        Vector3 movementUnit = movement.normalized;
        //recalibrate relative to joystick
        movementUnit = Quaternion.Euler(0, 180, 0) * movementUnit;

        if (movement.magnitude != 0f)
        {
            float magnitude = Mathf.Abs(movement.magnitude);
            if (magnitude > 1f) magnitude = 1f;

            movementUnit = Quaternion.Euler(0, theta, 0) * movementUnit;

            Vector3 planarMovement = movementUnit * speedScale * magnitude;
            velocity.x = planarMovement.x;
            velocity.z = planarMovement.z;
            
            //gameObject.transform.position += movementUnit * speedScale * magnitude;

            
            if (walkAge <= 0f)
            {
                heroAudio.Play(HeroAudio.Clip.WALK);
            }
            walkAge += Time.deltaTime;
            
            walkDuration = (1f/magnitude) * 0.3f;
            if (walkAge >= walkDuration)
            {
                walkAge = 0f;
            }

            if (lockedObject == null)
            {
                facing = movementUnit;
            }
            
            if (scaling&&platform==null)
            {
                facing = wallNormal;
            }
        }
        else
        {
            velocity.x = 0f;
            velocity.z = 0f;
            scaling = false;
            wall = null;
            heroAudio.Stop(HeroAudio.Clip.WALK);
 
        }
      

        speed = movement.magnitude;
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
                heroAudio.Play(HeroAudio.Clip.DASH);
                if (!GetComponent<CharacterController>().isGrounded)
                {
                    jumpAllowed = false;
                }

                if (!jumping) spawnPoint = gameObject.transform.position;
                else {
                    jumpDash++;
                }

                dashAge = 0f;
                dashAllowed = false;
                
               
                /*
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                GetComponent<Rigidbody>().AddForce(dashDirection * dashForce);
                GetComponent<Rigidbody>().useGravity = false;
                */

                
                
            }
        }
        else if (Input.GetButton("B"))
        {
            dashAge += Time.deltaTime;

            scaling = false;
            wall = null;

            Vector3 dashDirection = Vector3.zero;

            if (dashAge >= dashDuration)
            {
                stopDash();
                //dashAllowed = true;
            }
            else
            {
                
                if (lockedObject != null)
                {
                    //move towards locked object
                    Vector3 directionToTarget = lockedObject.transform.position - gameObject.transform.position;
                    directionToTarget.Normalize();
                    dashDirection = directionToTarget;

                }
                else {
                    
                    //move towards facing direction
                    if (scaling&&!dashing)
                    {
                        dashDirection = wallNormal;
                    }
                    else {
                        facing.Normalize();
                        dashDirection = facing;
                    }
                    
                }

                if (GetComponent<CharacterController>().isGrounded 
                    || (!GetComponent<CharacterController>().isGrounded && airDashAllowed))
                {
                    //GetComponent<CharacterController>().Move(dashDirection * dashForce * Time.deltaTime);
                    velocity += dashDirection * dashForce;
                }
      
            }

            //Debug.Log(dashDirection+ " "+scaling + " "+(wall== null)+" "+(dashAge<dashDuration));
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
        /*
        GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y,0);

        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
        */
        
    }

    private void stopJump()
    {
        jumping = false;
        /*
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
        */

    }
    private void doCharacterJump()
    {
        
        if (InputDeviceController.control.IsAPressed())
        {
            //Debug.Log(GetComponent<CharacterController>().isGrounded+" "+jumping+" "+jumpAllowed);
            if (!jumping&&jumpAllowed)
            {
                heroAudio.Play(HeroAudio.Clip.JUMP);
                
                jumpAllowed = false;
                standing = false;
                jumping = true;
                jumpAge = 0f;
                spawnPoint = gameObject.transform.position;
            }
        }
        else if (InputDeviceController.control.A)
        {
            if (jumping)
            {
                jumpAllowed = false;
                jumpAge += Time.deltaTime;

                //GetComponent<CharacterController>().Move(new Vector3(0, jumpForce * Time.deltaTime, 0));
                velocity.y += jumpForce;
                if (jumpAge >= jumpDuration)
                {
                    jumping = false;
                }
            }
        }
        else if (InputDeviceController.control.IsAReleased())
        {
            if(jumping) stopJump();
        }
    }
    private void GrabObject()
    {
        grabbing = true;
        GrabbingGameObject = TouchingGameObject;
        GrabbingGameObject.transform.SetParent(transform);
        GrabbingGameObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    private void LiftObject()
    {
        
        LiftingGameObject = TouchingGameObject;
        LiftingGameObject.GetComponent<Rigidbody>().isKinematic = true;
        LiftingGameObject.GetComponent<BoxCollider>().enabled = false;
        LiftingGameObject.transform.position = LiftingReference.transform.position;
        LiftingGameObject.transform.SetParent(LiftingReference.transform);
        LiftingGameObject.transform.localEulerAngles = Vector3.zero;
        lifting = true;
        Debug.Log("LIFTING " + LiftingGameObject.name);
    }
    private void DropObject()
    {

        //relieve existing object
        if (LiftingGameObject)
        {
            LiftingGameObject.GetComponent<Rigidbody>().isKinematic = false;
            LiftingGameObject.GetComponent<BoxCollider>().enabled = true;
            LiftingGameObject.transform.SetParent(transform.parent);
            Debug.Log("DROPPING " + LiftingGameObject.name);
            LiftingGameObject = null;

        }
        if (GrabbingGameObject)
        {
            GrabbingGameObject.transform.SetParent(transform.parent);
            GrabbingGameObject.GetComponent<Rigidbody>().isKinematic = false;
            GrabbingGameObject = null;
        }

        
        
    }

    private void ThrowObject()
    {
        Debug.Log("THROWING " + LiftingGameObject.name);
        //relieve existing object
        LiftingGameObject.GetComponent<Rigidbody>().isKinematic = false;
        LiftingGameObject.GetComponent<BoxCollider>().enabled = true;
        LiftingGameObject.transform.SetParent(transform.parent);
        LiftingGameObject.GetComponent<Rigidbody>().AddForce(facing * 1000f);
        LiftingGameObject = null;
        lifting = false;
        
    }
    
    private void doCharacterMainAction()
    {
        if (InputDeviceController.control.IsXPressed())
        {
            if (damageState == DamageState.DEAD) return;


            if (TouchingGameObject != null)
            {
                if (TouchingGameObject.GetComponent<Liftable>())
                {
                    //has a touching object
                    if (LiftingGameObject != null)
                    {
                        //throw if lifting something
                        ThrowObject();
                    }
                    else
                    {
                        //lift touching object
                        LiftObject();
                    }
                }
                else if (TouchingGameObject.GetComponent<Grabbable>())
                {
                    GrabObject();
                }
            }
            else
            {
                if (LiftingGameObject != null)
                {
                    //throw if lifting something
                    ThrowObject();
                }
                else
                {
                    if (weapon.GetComponent<Weapon>() != null)
                    {
                        shooting = true;
                        heroAudio.Play(HeroAudio.Clip.SHOT);
                        weapon.GetComponent<Weapon>().Fire();
                    }
                }


            }
        }
        else if (InputDeviceController.control.X)
        {
            if (!grabbing && !lifting) {
                charging = true;
            }
            
        }
        else if (InputDeviceController.control.IsXReleased())
        {
            shooting = false;
            charging = false;
            if (GrabbingGameObject)
            {
                GrabbingGameObject.transform.SetParent(transform.parent);
                GrabbingGameObject.GetComponent<Rigidbody>().isKinematic = false;
                GrabbingGameObject = null;
            }
        }
    }

    private void doRealignHMD()
    {
        if (Input.GetButtonDown("RB"))
        {
            hmdRig.GetComponent<QoobitOVR>().Realign(gameObject.transform.position,facing);
        }
        
    }

    private void doLockToTarget()
    {
        if (damageState == DamageState.DEAD) return;
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
