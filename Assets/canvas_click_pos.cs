using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class canvas_click_pos : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Canvas parentCanvas;        // the parent canvas of this UI - only needed to determine if we need the camera  
    [SerializeField] public RectTransform rect;         // the recttransform of the UI object


    // you can serialize this as well - do NOT assign it if the canvas render mode is overlay though
    public Camera UICamera;

    private void Start()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (UICamera == null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            UICamera = parentCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mp = new Vector2(mousePos.x, mousePos.y);
        Debug.Log("Event Data="+eventData.position.ToString());
        Debug.Log("Mouse Position=" + mp);
        // this UI element has been clicked by the mouse so determine the local position on your UI element
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, mp /*eventData.position*/, UICamera, out Vector2 localPos);
        Debug.Log("M Local Position (pre)=" + localPos.ToString());
        // we now have the local click position of our rect transform, but as you want the (0,0) to be bottom-left aligned, need to adjust it
        localPos.x += rect.rect.width / 2f;
        localPos.y += rect.rect.height / 2f;

        Debug.Log("M Local Position="+localPos.ToString());
    }
}
