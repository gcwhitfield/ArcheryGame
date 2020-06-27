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


    void Update()
    {
        if (CONTROL_OVERRIDE)
        {
            Vector3 moveVec = Vector3.zero;
            // movement
            if (Input.GetKey(forward))
            {
                moveVec += gameObject.transform.forward;
            }
            if (Input.GetKey(left))
            {
                moveVec -= gameObject.transform.right;
            }
            if (Input.GetKey(right))
            {
                moveVec += gameObject.transform.right;
            }
            if (Input.GetKey(back))
            {
                moveVec -= gameObject.transform.forward;
            }
            if (Input.GetKey(up))
            {
                moveVec += gameObject.transform.up;
            }
            if (Input.GetKey(down))
            {
                moveVec -= gameObject.transform.up;
            }

            gameObject.transform.Translate(moveVec * moveSpeed * Time.deltaTime, Space.Self);
           
            // rotation
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector3 rotVec = Vector3.zero;
            if (mouseX > 0)
            {
                rotVec += new Vector3(0, mouseX * rotationSpeed * Time.deltaTime, 0);
            }
            if (mouseY > 0)
            {
                rotVec += new Vector3(mouseX * rotationSpeed * Time.deltaTime, 0, 0);
            }
        }
    }
}
