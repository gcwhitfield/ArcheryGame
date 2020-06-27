using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("User Interface")]
    public GameObject HUD;
    public GameObject P1Health;
    public GameObject P2Health;

    public void UpdateHealth(LevelController.playerType player, float health)
    {
        switch(player)
        {
            case LevelController.playerType.P1:
                P1Health.GetComponent<HealthBar>().SetHealth(health);
                break;
            case LevelController.playerType.P2:
                P2Health.GetComponent<HealthBar>().SetHealth(health);
                break;
        }
    }
}
