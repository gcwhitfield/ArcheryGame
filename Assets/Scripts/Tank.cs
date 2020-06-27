using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

    1) Moving tanks
    2) Changing tank angle and rotation
    3) Changing power
    4) Shooting projectiles

*/
public class Tank : MonoBehaviour
{
    [Header("General")]
    public LevelController.playerType ptype; // P1 or P2
    // where the camera looks at the tank from
    [Header("UI and Location References")]
    public Transform closePos; // cam position
    public Transform farPos; // cam position 
    public GameObject nozzlePulledBack;
    public GameObject nozzle; // just the nozzle
    public GameObject nozzlePivotPoint;
    public GameObject nozzleGroup; // contains nozzle and top part of tank
    public GameObject tankUI;
    public GameObject cancelButtonUI;
    public GameObject moveButtonUI;
    public GameObject projectileInstantiationPosition;

    [Header("Health Settings")]
    public int health;
    private float _maxHealth = 100;

    [Header("Movement & Projectile Settings")]
    public float tankMoveSpeed;
    // angle of the arm
    public float angle;
    public float moveRange;
    public float shootPower;
    private bool _cancelMove;
    public GameObject bomb;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip bomSound;
    public AudioClip angleChangeSound;
    private bool _isMoving;

    private bool _isActiveMoveSequence;
    private GameObject _currCam;
    [SerializeField]
    private float _tankHeight;

    void Start()
    {
        _currCam = Camera.main.gameObject;
        _cancelMove = false;
        _isActiveMoveSequence = false;
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
    public void Damage(int amt)
    {
        Debug.Log("damage");
        health -= amt;
        UIManager.Instance.UpdateHealth(ptype, health/_maxHealth);
        if (health <= 0)
            Die();
    }

    public void DoMoveWrapper()
    {
        if (!_isActiveMoveSequence)
        {
            StartCoroutine("DoMove");
        }
    }

    /* The "Do" method gets called when we click on 
    a button on the UI. For example, when the user clicks on the "move" button,
    the "DoMove" method gets called */

    IEnumerator DoMove()
    {
        _isActiveMoveSequence = true;
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
           user can cancel move by calling _CancelMove fuction while this 
           loop is running
        */
        Ray ray;
        RaycastHit hit;
        while (true)
        {
            ray = _currCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 122, Color.yellow);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
            {
                if (Input.GetMouseButtonDown(0)) // left click
                {
                    // if the click is valid move location
                    if (Vector3.Distance(gameObject.transform.position, hit.point) <= moveRange)
                    {
                        // move the tank to the location
                        StartCoroutine("Move", hit.point + new Vector3(0, _tankHeight, 0));
                        // wait for the tank to move
                        while (_isMoving)
                            yield return null;
                        break;
                    }
                } else {
                    Debug.DrawLine(ray.origin, hit.point);
                }
            }
            if (_cancelMove)
            {
                break;
            }
            yield return null;
        }
        cancelButtonUI.SetActive(false);
        moveButtonUI.SetActive(true);
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsReturn);
        yield return new WaitForSeconds(0.1f);
       
        // wait for camera to finish moving
        while (LevelController.Instance.camIsMoving)
            yield return null;

        _isActiveMoveSequence = false;
        EndTurn();
        yield break;
    }

    /* Called from Cancel UI button. Cancels execution of DoMove coroutine */
    public void _CancelMove()
    {
        _cancelMove = true;
    }

    /* SetPower called from power slider in UI */
    public void SetPower()
    {

    }

    /* Called from the "Fire" button on the UI */
    public void Fire()
    {
        // instantiate projectile
        GameObject proj = Instantiate(bomb,projectileInstantiationPosition.transform.position, projectileInstantiationPosition.transform.rotation);
        proj.GetComponent<Bomb>().tank = gameObject;

        // add appropriate force proportional to power
        Vector3 direction = (projectileInstantiationPosition.transform.position - nozzle.transform.position).normalized;
        float _shootPower = 2;
        proj.GetComponent<Rigidbody>().AddForce(direction * _shootPower * shootPower, ForceMode.Impulse);

        // play the sound
        AudioManager.Instance.PlaySoundEffect(shootSound);
    }

    // changes the pitch of the arm by "amt" degrees
    public void ChangePitch(float angle)
    {
        nozzle.transform.position = nozzlePulledBack.transform.position;
        nozzle.transform.rotation = nozzlePulledBack.transform.rotation;
        nozzle.transform.RotateAround(nozzlePivotPoint.transform.position, nozzlePivotPoint.transform.right.normalized, angle);
    }

    // changes the yaw of the arm by "amt" degrees
    public void ChangeYaw(float angle)
    {
        nozzleGroup.transform.rotation = Quaternion.identity;
        nozzleGroup.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
    }


    protected Vector3 moveTo;
    // moves the tank to the given position
    IEnumerator Move(Vector3 pos)
    {
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
        _isMoving = false;
        yield return null;
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
    public void EndTurn()
    {
        // remove UI
        tankUI.SetActive(false);
        LevelController.Instance.SwitchTurn();
    }
}
