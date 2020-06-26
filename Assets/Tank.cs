using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    // where the camera looks at the tank from
    public Transform closePos;
    public Transform farPos;
    public LevelController.playerType ptype; // P1 or P2
    public GameObject tankUI;
    public GameObject currCam;
    public int health;
    // angle of the arm
    public float angle;
    public float moveRange;
    private bool isTurnOver;

    void Start()
    {
        currCam = Camera.main.gameObject;
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

    void DoMove()
    {
        Vector3 cameraPosOld = currCam.transform.position;
        // display the move cancel button

        // move camera to the far away location
        LevelController.CameraMoveParams camParamsFar = new LevelController.CameraMoveParams();
        camParamsFar.speed = 1;
        camParamsFar.destination = closePos.transform.position;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsFar);

        // move the camera to far position

        // wait for user to input desired location

        // move the tank to the location

        // wait for the tank to move

        // move the camera back
        LevelController.CameraMoveParams camParamsReturn = new LevelController.CameraMoveParams();
        camParamsReturn.speed = 1;
        camParamsReturn.destination = cameraPosOld;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsReturn);

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
    void Move(Vector3 pos)
    {
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
