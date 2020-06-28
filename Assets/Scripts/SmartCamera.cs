using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This should be attached to the main camera. 

Allow for user to override camera position with keyboard and mouse*/
public class SmartCamera : MonoBehaviour
{
    private bool CONTROL_OVERRIDE;
    public KeyCode forward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode back;
    public KeyCode up;
    public KeyCode down;
    public float moveSpeed;
    public float rotationSpeed;
    [Range(0,1)]
    public float overrideMoveSmoothAmt;
    private bool bforward;
    private bool bleft;
    private bool bright;
    private bool bback;
    private bool bup;
    private bool bdown;
    

    /* Sets the position and rotation of the camera (unless manual override) */
    public void SetPR(Vector3 position, Quaternion rotation)
    {
        if (!CONTROL_OVERRIDE)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
        }
    }

    /* Change position of cam (unless manual override) */
    public void SetPosition(Vector3 position)
    {
        if (!CONTROL_OVERRIDE)
        {
            gameObject.transform.position = position;
        }
    }
    
    /* Change rotation of cam (unless manual override) */
    public void SetRotation(Quaternion rotation)
    {
        if (!CONTROL_OVERRIDE)
        {
            gameObject.transform.rotation = rotation;
        }
    }

    /* Initiate keyboard movement of camera */
    public void BeginOverride()
    {
        CONTROL_OVERRIDE = true;
    }

    /* Go back to automatic camera movement */
    public void EndOverride()
    {   
        CONTROL_OVERRIDE = false;
    }   

    /* Look at position (unless manual override) */
    public void LookAt(Vector3 position)
    {
        if (!CONTROL_OVERRIDE)
        {
            gameObject.transform.LookAt(position);
        }
    }


    /* Begin camera movement override if the player presses any of the movement keys
        Rotate the camera while the user holds down right click.
     */
    void Update()
    {

        /*
        bforward = Input.GetKey(forward);
        bleft = Input.GetKey(left);
        bright = Input.GetKey(right);
        bup = Input.GetKey(up);
        bdown = Input.GetKey(down);
        bback = Input.GetKey(back);

        if (bforward || bleft || bright || bup || bdown || bback)
        {
            BeginOverride();
        }
        if (CONTROL_OVERRIDE)
        {
            Vector3 moveVec = Vector3.zero;
            // movement
            if (bforward)
            {
                moveVec -= gameObject.transform.forward;
            }
            if (bleft)
            {
                moveVec += gameObject.transform.right;
            }
            if (bright)
            {
                moveVec -= gameObject.transform.right;
            }
            if (bback)
            {
                moveVec += gameObject.transform.forward;
            }
            if (bup)
            {
                moveVec += gameObject.transform.up;
            }
            if (bdown)
            {
                moveVec -= gameObject.transform.up;
            }
            // smoothly move the camera
            gameObject.transform.position = 
                Vector3.Slerp(gameObject.transform.position, gameObject.transform.position + (moveVec * moveSpeed * Time.deltaTime), overrideMoveSmoothAmt);
            // rotation
            if (Input.GetMouseButton(1)) // right click to rotate camera
            {
                Cursor.lockState = CursorLockMode.Locked;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                Vector3 rotVec = Vector3.zero;
                if (!Mathf.Approximately(mouseX, 0))
                {
                    rotVec += new Vector3(0, mouseX * rotationSpeed * Time.deltaTime, 0);
                }
                /*
                if (!Mathf.Approximately(mouseY, 0))
                {
                    rotVec += new Vector3(mouseX * rotationSpeed * Time.deltaTime, 0, 0);
                }
                */
                /*
                gameObject.transform.Rotate(rotVec, Space.World);
            } else {
                Cursor.lockState = CursorLockMode.None;
            }
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        */
        
    }
}
