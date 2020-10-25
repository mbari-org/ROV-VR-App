using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler
{
    [SerializeField] public RectTransform dragRectTransform;
    public void OnDrag(PointerEventData eventData)
    {   // This function is called whenever the cursor is moved while the player is dragging 
        // Debug.Log("Dragging");

        // Delta contains movement from last mouse position
        dragRectTransform.anchoredPosition += eventData.delta;
        // Debug.Log(eventData.delta);
    }
}
