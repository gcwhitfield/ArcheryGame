using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singelton that controls what happens during the main gameplay loop
public class LevelController : Singleton<LevelController>
{
    [Header("Core")]
    public GameObject P1; // player 1
    public GameObject P2; // player 2
    public playerType currentTurn;
    // varaible will be accessed by WinScene to display the winner
    public winCondition currWinState = winCondition.NONE;
    [Header("Camera")]
    public SmartCamera smartCam;
    public bool camIsMoving;
    public float waitTimeInBetweenTurnSwitch;
    public enum playerType
    {
        P1,
        P2
    }

    public enum winCondition
    {
        P1_WIN,
        P2_WIN,
        TIE,
        NONE
    }
    void Awake()
    {
        smartCam = Camera.main.gameObject.GetComponent<SmartCamera>();
    }

    void Start()
    {
        camIsMoving = false;
        BeginTurn(currentTurn);
    }

    /* called at the start of every player's turn. This function is called by 
        Tank.cs
    */
    void BeginTurn(playerType player)
    {
        switch(player)
        {
            case playerType.P1:
                P1.GetComponent<Tank>().StartTurn();
                currentTurn = playerType.P1;
                break;
            case playerType.P2:
                P2.GetComponent<Tank>().StartTurn();
                currentTurn = playerType.P2;
                break;  
        }
    }

    // called when one of the players has won or if a tie has been reached
    public void GameOver(winCondition cond)
    {

        switch (cond)
        {
            case winCondition.P1_WIN:
                Debug.Log("PLayer 1 has won!");
                break;
            case (winCondition.P2_WIN):
                Debug.Log("Player 2 has won!");
                break;
        }

        // display the win scene
    }

    /* End P1's turn and begin P2's turn (or vice versa) */
    public void SwitchTurn()
    {
        StartCoroutine("_SwitchTurn");
    }

    IEnumerator _SwitchTurn()
    {
        yield return new WaitForSeconds(waitTimeInBetweenTurnSwitch);
        smartCam.EndOverride();
        switch(currentTurn)
        {
            case playerType.P1:
                Debug.Log("Switching turn from P1 to P2");
                BeginTurn(playerType.P2);
                break;
            case playerType.P2:
                Debug.Log("Switching turn from P2 to P1");
                BeginTurn(playerType.P1);
                break;
        }
        yield break;
    }

    public class CameraMoveParams
    {
        public float speed;
        public Vector3 destination;
        public Quaternion rotation;
    };
    /* Smoothly moves the camera to a new point
    
    Referenced Unity's documentation on Vector3.Slerp
    https://docs.unity3d.com/ScriptReference/Vector3.Slerp.html
     */
    public IEnumerator MoveCamera(CameraMoveParams camParams)
    {
        camIsMoving = true;
        float start = Time.time;
        if (smartCam == null)
        {
            smartCam = Camera.main.gameObject.GetComponent<SmartCamera>();
        }
        Vector3 startPos = smartCam.transform.position;
        Quaternion startRot = smartCam.transform.rotation;
        float fracComplete = 0;
        while (fracComplete < 1)
        {
            // slerp between the positions
            fracComplete = ((Time.time - start) * camParams.speed) / Vector3.Distance(startPos, camParams.destination);
            smartCam.SetPR(
                Vector3.Slerp(startPos, camParams.destination, fracComplete),
                Quaternion.Slerp(startRot, camParams.rotation, fracComplete));

            yield return null;
        }
        camIsMoving = false;
        yield break;
    }
}
