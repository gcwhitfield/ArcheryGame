using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject foreground;
    private float health; // percentage

    /* Add health to the progress bar */
    public void AddHealth(float percent)
    {
        health += percent;
        ApplyHealthToUI();
    }
    /* Subtracts health from the progress bar */
    public void SubtractHealth(float percent)
    {
        health -= percent;
        ApplyHealthToUI();
    }

    /* Sets the health percentage to given value. Must be between
    0 and 1 (inclusive) */
    public void SetHealth(float percent)
    {
        percent = Mathf.Clamp(percent, 0, 1);
        health = percent;
        ApplyHealthToUI();
    }

    void ApplyHealthToUI()
    {
        RectTransform rtrans = foreground.GetComponent<RectTransform>();
        rtrans.localScale = new Vector3(health, rtrans.localScale.y, rtrans.localScale.z);
    }
}
