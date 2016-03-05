using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class InputDeviceController : MonoBehaviour {
    public static InputDeviceController control;        //singleton
    public float HorizontalAxis;
    public float VerticalAxis;
    public bool A;
    public bool B;
    public bool X;
    public bool Y;
    public bool LB;
    public float LT;
    public bool RB;
    public float RT;
    public bool SelectButton;
    public bool StartButton;
    

    protected bool APressed;
    protected bool AReleased;
    protected bool XPressed;
    protected bool XReleased;
    protected bool YPressed;
    protected bool YReleased;
    protected bool BPressed;
    protected bool BReleased;
    protected bool RBPressed;
    protected bool RBReleased;
    protected bool LBPressed;
    protected bool LBReleased;

    public bool IsAPressed() { return APressed; }
    public bool IsAReleased() { return AReleased; }
    public bool IsXPressed() { return XPressed; }
    public bool IsXReleased() { return XReleased; }


    public KeyCode UpKey = KeyCode.W;
    public KeyCode DownKey = KeyCode.S;
    public KeyCode LeftKey = KeyCode.A;
    public KeyCode RightKey = KeyCode.D;

    public KeyCode Fire1Key = KeyCode.R;
    public KeyCode JumpKey = KeyCode.Space;

    public KeyCode DashKey = KeyCode.E;
    public KeyCode OtherKey = KeyCode.F;

    
    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }


    void Start () {

        HorizontalAxis = VerticalAxis = 0f;
      
        APressed = AReleased = XPressed = XReleased = false;
        A = B = X = Y = RB = LB = SelectButton = StartButton = false;
        LT = RT = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw("LeftJoystickX") != 0f)
        {
            HorizontalAxis = Input.GetAxisRaw("LeftJoystickX");
        }
        else
        {
            //keyboard controls
            HorizontalAxis = 0f;
            if (Input.GetKey(LeftKey))
            {
                HorizontalAxis -= 1f;
            }
            else if (Input.GetKey(RightKey))
            {
                HorizontalAxis += 1f;
            }

        }
        if (Input.GetAxisRaw("LeftJoystickY") != 0f)
        {
            VerticalAxis = Input.GetAxisRaw("LeftJoystickY");
        }
        else
        {
            //keyboard controls
            VerticalAxis = 0f;
            if (Input.GetKey(UpKey))
            {
                VerticalAxis -= 1f;
            }
            else if (Input.GetKey(DownKey))
            {
                VerticalAxis += 1f;
            }
        }

        #region A Button
        if (Input.GetButtonDown("A") || Input.GetKeyDown(JumpKey) )
        {
            APressed = true;
        }
        else
        {
            APressed = false;
        }
        if (Input.GetButton("A") || Input.GetKey(JumpKey))
        {
            A = true;
        }
        else
        {
            A = false;
        }
        if (Input.GetButtonUp("A") || Input.GetKeyUp(JumpKey))
        {
            AReleased = true;
        }
        else
        {
            AReleased = false;
        }
        #endregion
        #region B Button
        if (Input.GetButtonDown("B") || Input.GetKeyDown(DashKey) )
        {
            BPressed = true;
        }
        else
        {
            BPressed = false;
        }
        if (Input.GetButton("B") || Input.GetKey(DashKey))
        {
            B = true;
        }
        else
        {
            B = false;
        }
        if (Input.GetButtonUp("B") || Input.GetKeyUp(DashKey))
        {
            BReleased = true;
        }
        else
        {
            BReleased = false;
        }
        #endregion
        #region Y Button
        if (Input.GetButtonDown("Y") || Input.GetKeyDown(OtherKey))
        {
            YPressed = true;
        }
        else
        {
            YPressed = false;
        }
        if (Input.GetButton("Y") || Input.GetKey(OtherKey))
        {
            Y = true;
        }
        else
        {
            Y = false;
        }
        if (Input.GetButtonUp("Y") || Input.GetKeyUp(OtherKey))
        {
            YReleased = true;
        }
        else
        {
            YReleased = false;
        }
        #endregion
        #region X Button
        if (Input.GetButtonDown("X") || Input.GetKeyDown(Fire1Key))
        {
            XPressed = true;
        }
        else
        {
            XPressed = false;
        }
        
        if (Input.GetButton("X") || Input.GetKey(Fire1Key))
        {
            X = true;
        }
        else
        {
            X = false;
        }
        if (Input.GetButtonUp("X") || Input.GetKeyUp(Fire1Key))
        {
            XReleased = true;
        }
        else
        {
            XReleased = false;
        }
        #endregion
        #region LB Button
        if (Input.GetButtonDown("LB"))
        {
            LBPressed = true;
        }
        else
        {
            LBPressed = false;
        }
        if (Input.GetButton("LB"))
        {
            LB = true;
        }
        else
        {
            LB = false;
        }
        if (Input.GetButtonUp("LB"))
        {
            LBReleased = true;
        }
        else
        {
            LBReleased = false;
        }
        #endregion
        #region RB Button
        if (Input.GetButtonDown("RB"))
        {
            RBPressed = true;
        }
        else
        {
            RBPressed = false;
        }
        if (Input.GetButton("RB"))
        {
            RB = true;
        }
        else
        {
            RB = false;
        }
        if (Input.GetButtonUp("RB"))
        {
            RBReleased = true;
        }
        else
        {
            RBReleased = false;
        }
        #endregion

        #region LT Button

        if (Input.GetAxisRaw("LT") > 0f)
        {
            LT = Input.GetAxisRaw("LT");
        }
        else
        {
            LT = 0f;
        }
        #endregion
        #region RT Button

        if (Input.GetAxisRaw("RT") > 0f)
        {
            RT = Input.GetAxisRaw("RT");
        }
        else
        {
            RT = 0f;
        }
        #endregion
       
    }
}
