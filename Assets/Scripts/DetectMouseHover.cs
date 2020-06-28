/* Copied from unity Documentation on using IPointerEventHandler 
https://docs.unity3d.com/2017.4/Documentation/ScriptReference/EventSystems.IPointerEnterHandler.html */

//Attach this script to the GameObject you would like to have mouse hovering detected on
//This script outputs a message to the Console when the mouse pointer is currently detected hovering over the GameObject and also when the pointer leaves.

using UnityEngine;
using UnityEngine.EventSystems;

public class DetectMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseHovering;
    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMouseHovering = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMouseHovering = false;
    }
}