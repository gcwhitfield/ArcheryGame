using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("General")]
    public LevelController.playerType ptype; // P1 or P2
    // where the camera looks at the tank from
    [Header("UI and Location References")]
    public Transform closePos;
    public Transform farPos;
    public GameObject tankUI;
    public GameObject nozzlePulledBack;
    public GameObject nozzle;
    public GameObject nozzlePivotPoint;
    public GameObject cancelButton;
    public GameObject moveButton;

    [Header("Health Settings")]
    public int health;

    [Header("Movement & Projectile Settings")]
    public float tankMoveSpeed;
    // angle of the arm
    public float angle;
    public float moveRange;
    public float shootPower;
    private bool cancelMove;
    public GameObject bomb;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip bomSound;
    public AudioClip angleChangeSound;
    private bool isMoving;

    private bool isActiveMoveSequence;
    private GameObject currCam;
    [SerializeField]
    private float tankHeight;

    void Start()
    {
        currCam = Camera.main.gameObject;
        cancelMove = false;
        isActiveMoveSequence = false;
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

    public void DoMoveWrapper()
    {
        if (!isActiveMoveSequence)
        {
            Debug.Log("begin DoMove coroutine");
            StartCoroutine("DoMove");
        }
    }

    /* The "Do" methods are the ones that get called when we click on 
    a button on the UI. For example, when the user clicks on the "move" button,
    the "DoMove" method gets called */

    IEnumerator DoMove()
    {
        isActiveMoveSequence = true;
        Debug.Log("being Do Move");
        // display the move cancel button

        // for moving camera to the far away location
        LevelController.CameraMoveParams camParamsFar = new LevelController.CameraMoveParams();
        camParamsFar.speed = 50;
        camParamsFar.destination = farPos.transform.position;
        camParamsFar.rotation = farPos.transform.rotation;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsFar);

        // for moving the camera back
        LevelController.CameraMoveParams camParamsReturn = new LevelController.CameraMoveParams();
        camParamsReturn.speed = 50;
        camParamsReturn.destination = closePos.position;
        camParamsReturn.rotation = closePos.rotation; 

        /* wait for user to input desired location
           user can cancel move by calling CancelMove fuction while this 
           loop is running
        */
        Ray ray;
        RaycastHit hit;
        Debug.Log("raycast part");
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
                        Debug.Log("valid move location. moving...");
                        // move the tank to the location
                        StartCoroutine("Move", hit.point + new Vector3(0, tankHeight, 0));
                        // wait for the tank to move
                        while (isMoving)
                            yield return null;
                        break;
                    }
                } else {
                    Debug.DrawLine(ray.origin, hit.point);
                }
            }
            if (cancelMove)
            {
                break;
            }
            yield return null;
        }
        cancelButton.SetActive(false);
        moveButton.SetActive(true);
        Debug.Log("finish moving");
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsReturn);
        isActiveMoveSequence = false;
        yield break;
    }

    /* Called from Cancel UI button */
    public void CancelMove()
    {
        cancelMove = true;
    }


    void DoPower()
    {

    }


    // changes the angle of the arm by "amt" degrees
    public void ChangeAngle(float angle)
    {
        nozzle.transform.position = nozzlePulledBack.transform.position;
        nozzle.transform.rotation = nozzlePulledBack.transform.rotation;
        nozzle.transform.RotateAround(nozzlePivotPoint.transform.position, nozzlePivotPoint.transform.right.normalized, angle);
    }



    protected Vector3 moveTo;
    // moves the tank to the given position
    IEnumerator Move(Vector3 pos)
    {
        Debug.Log("Moving the player to " + pos.ToString());
        float start = Time.time;
        float fracComplete = 0;
        Vector3 startPos = transform.position;
        gameObject.transform.LookAt(pos);
        while (fracComplete < 1)
        {
            // lerp between the positions
            fracComplete = ((Time.time - start) * tankMoveSpeed) / Vector3.Distance(startPos, pos);
            gameObject.transform.position = Vector3.Slerp(startPos, pos, fracComplete);
            yield return null;
        }
        isMoving = false;
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
    public void StartTurn()
    {
        // move the main camera to the tank's camera view location
        float cameraMoveSpeed = 1;

        LevelController.CameraMoveParams camParams = new LevelController.CameraMoveParams();
        camParams.speed = 50;
        camParams.destination = closePos.transform.position;
        camParams.rotation = closePos.transform.rotation;
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
