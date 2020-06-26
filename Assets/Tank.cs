﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    // where the camera looks at the tank from
    public Transform closePos;
    public Transform farPos;
    public LevelController.playerType ptype; // P1 or P2
    public GameObject tankUI;
    public float tankMoveSpeed;
    public int health;
    // angle of the arm
    public float angle;
    public float moveRange;
    private bool cancelMove;
    private GameObject currCam;
    [SerializeField]
    private float tankHeight;

    void Start()
    {
        currCam = Camera.main.gameObject;
        cancelMove = false;
    }
    // called when the tank has no health
    void Die()
    {
        switch(ptype)
        {
            case LevelController.playerType.P1:
                LevelController.Instance.GameOver(LevelController.winCondition.P2_WIN);
                break;
            case LevelController.playerType.P2:
                LevelController.Instance.GameOver(LevelController.winCondition.P1_WIN);
                break;
        }

    }

    // subtract health from the tank.
    void Damage(int amt)
    {
        health -= amt;
        if (health <= 0)
            Die();
    }

    /* The "Do" methods are the ones that get called when we click on 
    a button on the UI. For example, when the user clicks on the "move" button,
    the "DoMove" method gets called */

    IEnumerator DoMove()
    {
        Vector3 cameraPosOld = currCam.transform.position;
        // display the move cancel button

        // move camera to the far away location
        LevelController.CameraMoveParams camParamsFar = new LevelController.CameraMoveParams();
        camParamsFar.speed = 1;
        camParamsFar.destination = closePos.transform.position;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsFar);

        /* wait for user to input desired location
        
           user can cancel move by calling CancelMove fuction while this 
           loop is running
        */
        Ray ray;
        RaycastHit hit;
        while (true)
        {
            ray = currCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 122, Color.yellow);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
            {
                if (Input.GetMouseButtonDown(0)) // left click
                {
                    // if the click is valid move location
                    if (Vector3.Distance(gameObject.transform.position, hit.point) <= moveRange)
                    {
                        Move(hit.point);
                        break;
                    }
                } else {
                    Debug.DrawLine(ray.origin, hit.point);
                }
            }
            if (cancelMove)
            {
                cancelMove = false;
                yield break;
            }
            yield return null;
        }

        Debug.Log("Moving tank to the location");
        // move the tank to the location
        Move(hit.point + new Vector3(0, tankHeight, 0));

        // wait for the tank to move

        // move the camera back
        LevelController.CameraMoveParams camParamsReturn = new LevelController.CameraMoveParams();
        camParamsReturn.speed = 1;
        camParamsReturn.destination = cameraPosOld;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsReturn);
    }

    void CancelMove()
    {
        cancelMove = true;
    }

    void DoAngle()
    {

    }

    void DoPower()
    {

    }


    // changes the angle of the arm by "amt" degrees
    void ChangeAngle(float amt)
    {
        float animationDuration = 1; // in seconds

       // smoothly lerp between original and new angle
    }



    protected Vector3 moveTo;
    // moves the tank to the given position
    IEnumerable Move(Vector3 pos)
    {
        Debug.Log("Moving the player to " + pos.ToString());
        float start = Time.time;
        float fracComplete = 0;
        Vector3 startPos = transform.position;
        while (fracComplete < 1)
        {
            // lerp between the positions
            transform.position = Vector3.Lerp(startPos, pos, fracComplete);
            fracComplete = (Time.time - start / start) * tankMoveSpeed;
            yield return null;
        }
        yield return null;
    }

    /* 
        Sets the camera position back to default
        Sets the UI back to default
    */
    void ResetPlayerState()
    {
        Camera.main.transform.position = farPos.position;
        Camera.main.transform.rotation = farPos.rotation;

    }

    /*
    Starts the player's turn sequence. The player can do the following things
    during their turn:

    1) move the tank
    2) change the angle
    3) choose a weapon to shoot
    4) shoot the enemy

    Moving or shooting will use up your turn
    Changing the angle does NOT count as a use of your turn
    */
    void StartTurn()
    {
        // move the main camera to the tank's camera view location
        float cameraMoveSpeed = 1;

        LevelController.CameraMoveParams camParams = new LevelController.CameraMoveParams();
        camParams.speed = 1;
        camParams.destination = closePos.transform.position;
        LevelController.Instance.StartCoroutine("MoveCamera", camParams);

        // dispay the turn UI
        tankUI.SetActive(true);
    }

    void EndTurn()
    {
        // remove UI
        tankUI.SetActive(false);
    }
}
