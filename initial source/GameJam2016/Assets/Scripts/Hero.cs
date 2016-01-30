using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HeroState { IDLE, WALKING, JUMPING, DASHING, AIR_DASHING, SHOOTING };
public class Hero : MonoBehaviour {
    public GameObject hero;
    public Vector3 forward;
    HeroState state = HeroState.IDLE;
    bool onFloor;
    public Vector3 facing;

    List<GameObject> platforms = new List<GameObject>();
    List<GameObject> bullets = new List<GameObject>();
    Object bulletObject;
    float speed;
    

    Vector3 lastVelocity;

    float lastRT;
	// Use this for initialization
	void Start () {
        facing = new Vector3(0, 0, 1);
        speed = 0.1f;
        
        Physics.gravity = new Vector3(0, -50, 0);
        bulletObject = Resources.Load("Projectile", typeof(GameObject));

        platforms.Add(GameObject.Find("Gun_Barrel"));
        platforms.Add(GameObject.Find("platform"));
        platforms.Add(GameObject.Find("platform (1)"));
        platforms.Add(GameObject.Find("platform (2)"));
        platforms.Add(GameObject.Find("platform (3)"));
        platforms.Add(GameObject.Find("platform (4)"));
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Terrain")
        {
            //Debug.Log("on terrain");
            state = HeroState.IDLE;
            onFloor = true;
        }
        else
        {
            for(int i = 0; i < platforms.Count; i++)
            {
                if(col.gameObject.name == platforms[i].name)
                {
                    state = HeroState.IDLE;
                    onFloor = true;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {

        forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();



        Vector3 movement = new Vector3(-Input.GetAxisRaw("LeftJoystickX"), 0, Input.GetAxisRaw("LeftJoystickY"));
        //Debug.Log(movement);
        movement.Normalize();
        movement = Quaternion.Euler(0, 180, 0) * movement;



        float theta = Vector3.Angle(Vector3.forward, forward);
        theta *= Mathf.Sign(Vector3.Cross(Vector3.forward, forward).y);


        if (movement.magnitude != 0f)
        {

            movement = Quaternion.Euler(0, theta, 0) * movement;

            hero.transform.position += movement * speed;
            state = HeroState.WALKING;
            facing = movement;



        }
        else
        {
            if (state == HeroState.WALKING)
            {
                state = HeroState.IDLE;
            }
        }

        

        Vector3 direction = new Vector3(-Input.GetAxisRaw("RightJoystickX"), 0, Input.GetAxisRaw("RightJoystickY"));
        //Debug.Log(movement);
        direction.Normalize();
        direction = Quaternion.Euler(0, 180, 0) * direction;



        if (direction.magnitude != 0f)
        {


            facing = Quaternion.Euler(0, theta, 0) * direction;

            //facing = direction;
        }

        float lookAngle = Vector3.Angle(Vector3.forward, facing);
        lookAngle *= Mathf.Sign(Vector3.Cross(Vector3.forward, facing).y);
        //Debug.Log(Vector3.forward + " " + direction + " " + "ANGLE:" + lookAngle);
        hero.transform.localEulerAngles = new Vector3(0, lookAngle, 0);

        
        if (Input.GetButtonDown("A"))
        {
            if ((state == HeroState.IDLE || state == HeroState.WALKING) && onFloor)
            {
                hero.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000f);
                state = HeroState.JUMPING;
                onFloor = false;
                Debug.Log("LEAVING");
            }
        }
        /*
        if (Input.GetButtonDown("RB"))
        {

        }
        */

        if (Input.GetAxisRaw("RT") < 0f && lastRT == 0f)
        {
            //down
            GameObject bullet = Instantiate(bulletObject, gameObject.transform.position, Quaternion.identity) as GameObject;
            bullet.GetComponent<Projectile>().direction = facing;
            //bullet.transform.parent = hero.transform;
            bullet.name = "Projectile";

        }
        lastRT = Input.GetAxisRaw("RT");

        if (hero.GetComponent<Rigidbody>().velocity.y == 0f)
        {
            //Debug.Log(lastVelocity);
        }
        
        
        
        lastVelocity = hero.GetComponent<Rigidbody>().velocity;

        //Oculus look vector
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * 100f),Color.red);

        //facing vector
        Debug.DrawLine(hero.transform.position,hero.transform.position + (facing * 100f), Color.green);
    }
}
