using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singelton that controls what happens during the main gameplay loop
public class LevelController : Singleton<LevelController>
{
    public GameObject P1; // player 1
    public GameObject P2; // player 2

    // varaible will be accessed by WinScene to display the winner
    public winCondition currWinState = winCondition.NONE;
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
    void Start()
    {

    }

    /* called at the start of every player's turn. This function is called by 
        Tank.cs
    */
    void BeginTurn(playerType player)
    {
        switch(player)
        {
            case playerType.P1:

                break;
            case playerType.P2:
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



    public class CameraMoveParams
    {
        public float speed;
        public Vector3 destination;
    };
    /* Smoothly moves the camera to a new point
    
    Referenced Unity's documentation on Vector3.Slerp
    https://docs.unity3d.com/ScriptReference/Vector3.Slerp.html
     */
    public IEnumerator MoveCamera(CameraMoveParams camParams)
    {
        float start = Time.time;
        float fracComplete = 0;
        while (fracComplete < 1)
        {
            // slerp between the positions
            fracComplete = (Time.time - start / start);
            yield return null;
        }

    }
}
