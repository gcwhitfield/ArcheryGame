using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
    Code found on unity tutorial website:
    https://nagachiang.github.io/tutorial-pass-parameter-with-sliders-onvaluechanged-in-unity/#
*/
[RequireComponent(typeof(Text))]
public class SliderValueChangedDisplay : MonoBehaviour
{
    public Text text;
    public bool showDecimals;

    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    public void UpdateText(float value)
    {
        if (showDecimals)
        {
            text.text = value.ToString("0.0");
        } else {
            text.text = value.ToString("0");
        }
    }
}
