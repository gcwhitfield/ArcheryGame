using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject nozzleGroupOriginalRotation;
    public GameObject nozzle; // just the nozzle
    public GameObject nozzlePivotPoint;
    public GameObject nozzleGroup; // contains nozzle and top part of tank
    public GameObject tankUI;
    public GameObject cancelButtonUI;
    public GameObject moveButtonUI;
    public GameObject projectileInstantiationPosition;
    public GameObject mainHUD;
    public Slider pitchSlider;

    [Header("Health Settings")]
    public int health;
    private float _maxHealth = 100;

    
    [Header("Movement and Projectile Settings")]
    public float tankMoveSpeed;
    private float _tankHeight;
    // angle of the arm
    public float pitch;
    public float yaw;
    public float moveRange;
    public float power;
    private bool _cancelMove;
    public GameObject bomb;
    public LineRenderer circleLR;
    public Material moveValid;
    public Material moveInvalid;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip moveSound;
    public AudioClip angleChangeSound;
    private bool _isMoving;

    private bool _isActiveMoveSequence;
    [SerializeField]

    void Start()
    {
        _cancelMove = false;
        _isActiveMoveSequence = false;
        ChangePitch(pitchSlider.value);
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

        CalculateMovementCircle();
        SetCamFar();

        /* wait for user to input desired location
           user can cancel move by calling _CancelMove fuction while this 
           loop is running
        */
        Ray ray;
        RaycastHit hit;
        while (true)
        {
            ray = LevelController.Instance.smartCam.gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 122, Color.yellow);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
            {
                // if the click is valid move location
                if (Vector3.Distance(gameObject.transform.position, hit.point) <= moveRange)
                {
                    ShowMovementCircle(true);
                    if (Input.GetMouseButtonDown(0)) // left click
                    {
                        if (!(tankUI.GetComponent<DetectMouseHover>().isMouseHovering))
                        {
                            // move the tank to the location
                            StartCoroutine("Move", hit.point + new Vector3(0, _tankHeight, 0));
                            HideMovementCircle();
                            // wait for the tank to move
                            while (_isMoving)
                                yield return null;
                            break;
                        }
                    }
                } else { // invalid location
                    ShowMovementCircle(false);
                }
            }
            if (_cancelMove)
            {
                Debug.Log("cancel move");
                HideMovementCircle();
                break;
            }
            yield return null;
        }

        
        cancelButtonUI.SetActive(false);
        moveButtonUI.SetActive(true);
        SetCamClose();
        yield return null;
       
        // wait for camera to finish moving
        while (LevelController.Instance.camIsMoving)
            yield return null;

        _isActiveMoveSequence = false;
        if (!_cancelMove)
            EndTurn();
        _cancelMove = false;
        yield break;
    }

    /* Called from Cancel UI button. Cancels execution of DoMove coroutine */
    public void CancelMove()
    {
        Debug.Log("Cancel move OUTER");
        _cancelMove = true;
    }

    /* Sets vertex positions of circle movement line renderer */
    void CalculateMovementCircle()
    {
        int numPoints = 30;

        circleLR.positionCount = numPoints + 1;
        for (int i = 0; i < numPoints + 1; i ++)
        {
            float angle = (i * 2 * Mathf.PI) / numPoints;
            float offsetX = Mathf.Sin(angle) * moveRange;
            float offsetZ = Mathf.Cos(angle) * moveRange;
            Vector3 vertPosOffset = new Vector3 (offsetX, _tankHeight, offsetZ);
            circleLR.SetPosition(i, vertPosOffset + gameObject.transform.position);
        }
    }

    /* Created a circle around the tank showing where movement can happen.
    Displays valid movement circle if isValid is true, invalid circle if false
    */
    void ShowMovementCircle(bool isValid)
    {
        if (isValid)
        {
            circleLR.material = moveValid;
        } else {
            circleLR.material = moveInvalid;
        }
        circleLR.gameObject.SetActive(true);
    }

    void HideMovementCircle()
    {
        circleLR.gameObject.SetActive(false);
    }


    /* SetPower called from power slider in UI */
    public void SetPower(float newPower)
    {
        power = newPower;
    }

    /* Called from the "Fire" button on the UI */
    public void Fire()
    {
        LevelController.Instance.smartCam.EndOverride();
        // instantiate projectile
        GameObject proj = Instantiate(bomb,projectileInstantiationPosition.transform.position, projectileInstantiationPosition.transform.rotation);
        proj.GetComponent<Bomb>().tank = gameObject;

        // add appropriate force proportional to power
        Vector3 direction = (projectileInstantiationPosition.transform.position - nozzle.transform.position).normalized;
        float _power = 0.6f;
        proj.GetComponent<Rigidbody>().AddForce(direction * _power * power, ForceMode.Impulse);

        // play the sound
        AudioManager.Instance.PlaySoundEffect(shootSound);
    }

    // changes the pitch of the arm by "amt" degrees
    public void ChangePitch(float angle)
    {
        pitch = angle;
        nozzle.transform.position = nozzlePulledBack.transform.position;
        nozzle.transform.rotation = nozzlePulledBack.transform.rotation;
        nozzle.transform.RotateAround(nozzlePivotPoint.transform.position, nozzlePivotPoint.transform.right.normalized, angle);
    }

    // changes the yaw of the arm by "amt" degrees
    public void ChangeYaw(float angle)
    {
        yaw = angle;
        nozzleGroup.transform.rotation = nozzleGroupOriginalRotation.transform.rotation;
        nozzleGroup.transform.Rotate(new Vector3(0, angle, 0), Space.Self);
        LevelController.Instance.smartCam.EndOverride();
        LevelController.Instance.smartCam.SetPR(closePos.transform.position, closePos.transform.rotation);

    }

    /* Sets the position of the camera to Close */
    public void SetCamClose()
    {
        LevelController.Instance.smartCam.EndOverride();
        LevelController.CameraMoveParams camParamsClose = new LevelController.CameraMoveParams();
        camParamsClose.speed = 160;
        camParamsClose.destination = closePos.position;
        camParamsClose.rotation = closePos.rotation; 
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsClose);
    }

    /* Sets the positon of the camera to Far  */
    public void SetCamFar()
    {
        LevelController.Instance.smartCam.EndOverride();
        LevelController.CameraMoveParams camParamsFar = new LevelController.CameraMoveParams();
        camParamsFar.speed = 160;
        camParamsFar.destination = farPos.transform.position;
        camParamsFar.rotation = farPos.transform.rotation;
        LevelController.Instance.StartCoroutine("MoveCamera", camParamsFar);
    }
    protected Vector3 moveTo;
    // moves the tank to the given position
    IEnumerator Move(Vector3 pos)
    {
        float start = Time.time;
        float fracComplete = 0;
        AudioManager.Instance.effectsSource.loop = true;
        AudioManager.Instance.PlaySoundEffect(moveSound);
        Vector3 startPos = transform.position;
        gameObject.transform.LookAt(pos);
        while (fracComplete < 1)
        {
            // lerp between the positions
            fracComplete = ((Time.time - start) * tankMoveSpeed) / Vector3.Distance(startPos, pos);
            gameObject.transform.position = Vector3.Lerp(startPos, pos, fracComplete);
            yield return null;
        }
        _isMoving = false;
        AudioManager.Instance.StopSoundEffect();
        AudioManager.Instance.effectsSource.loop = false;
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
        LevelController.CameraMoveParams camParams = new LevelController.CameraMoveParams();
        camParams.speed = 160;
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
        mainHUD.SetActive(true);
        LevelController.Instance.SwitchTurn();
    }
}
