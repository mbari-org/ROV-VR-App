using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera m_Camera; // Camera attached to controller
    public SteamVR_Input_Sources m_VRTargetSource; // Left or right controller
    public SteamVR_Action_Boolean m_VRClickAction;
    public string m_KeyboardClickKey = "space"; // Equivalent key for foot pedal

    // Private pointers for our data
    private GameObject m_CurrentObject = null;
    private PointerEventData m_Data = null;
    private Vector3 m_lastRaycastPosition;

    // Used for double-click detection
    private bool singleTap = false;
    private bool doubleTap = false;
    private bool tapping = false;
    private float tapTime = 0;
    private float duration = .4f;

    protected override void Awake()
    {
        // We are overriding the awake that's happening within the base input module
        // We want to call any functionaly that may be in there
        base.Awake();

        // We have to set up our pointer event data
        m_Data = new PointerEventData(eventSystem);
    }

    public override void Process() 
    {
        // Runs all the time - anything we want to do regularly we put here
        // Within every frame, we will update our pointer event data so that other things in our project have access to them
        // We may want to write a class that inherits from pointer event data for more complex project

        // Reset data (we might be doing things in ProcessPress and ProcessRelease we don't want to carry over) & set camera
        m_Data.Reset();
        // Get middle of camera 
        m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2); // Gets amount of pixels wide that we have for our camera

        // Raycast from the middle of the controller camera
        eventSystem.RaycastAll(m_Data, m_RaycastResultCache); 
        m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache); // We want the first object hit 
        m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;

        // Delta is used to measure how much the cursor was moved. We will use it for drag (we'll store world distance between the last and current raycasts) 
        Vector3 hitPoint = m_Data.pointerCurrentRaycast.worldPosition;
        // Create vector representing x and z componenets 
        // TODO: Make this work for both X and Z
        // Vector2 xzVector = new Vector2(hitPoint.x - m_lastRaycastPosition.x, hitPoint.z - m_lastRaycastPosition.z);
        // Compute delta
        m_Data.delta = new Vector2(hitPoint.x - m_lastRaycastPosition.x, hitPoint.y - m_lastRaycastPosition.y);
        // Debug.Log(m_Data.delta);
        m_lastRaycastPosition = hitPoint;

        // Clear raycast cache
        m_RaycastResultCache.Clear();

        // Handle our hover states 
        HandlePointerExitAndEnter(m_Data, m_CurrentObject);

        // Trigger click if either controllers or keyboard key/foot pedal is pressed
        bool isDown = m_VRClickAction.GetStateDown(m_VRTargetSource) ^ Input.GetKeyDown(m_KeyboardClickKey);
        bool isUp = m_VRClickAction.GetStateUp(m_VRTargetSource) ^ Input.GetKeyUp(m_KeyboardClickKey);

        if(isDown) ProcessPress(m_Data);
        if(isUp) ProcessRelease(m_Data);

        // Determine whether click is a single or double click
        if (isDown) {
            if (tapping) {
                doubleTap = true;
                // Debug.Log("Double-Click");
                tapping = false;
            } else {
                tapping = true;
                tapTime = duration;
            }
        }
        if (tapping) {
            tapTime = tapTime - Time.deltaTime;
            if (tapTime <= 0) {
                tapping = false;
                singleTap = true;
                // Debug.Log("Single-Click");
            }
        }

        // Handle Dragging
        ExecuteEvents.Execute(m_Data.pointerDrag, m_Data, ExecuteEvents.dragHandler);
    }
     void LateUpdate()
     {
         if (doubleTap) doubleTap = false;
         if (singleTap) singleTap = false;
     }

    public PointerEventData GetData()
    {    
        // Gets data by updating our pointers
        // Gets distance to update the pointer
        return m_Data;
    }

    private void ProcessPress(PointerEventData data)
    {
        // For more details on other usable things, look at definition for PointerEventData

        // Set raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        // Check for object hit, get the down handler, call down handler (for buttons)
        data.pointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);
        data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(data.pointerCurrentRaycast.gameObject);

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.beginDragHandler);
    }

    private void ProcessRelease(PointerEventData data)
    {
        // Check for click handler
        // when we release our press let's get the object that is currently being released up on so we can compare
        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

        // Check if the object pressed previously and the one we're releasing now are the same 
        if(data.pointerPress == pointerRelease) 
            // If they are the same, actually execute the "click"
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);

        // Execute pointer release
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.endDragHandler);

        data.pointerPress = null;
        data.pointerDrag = null;
        data.pointerCurrentRaycast.Clear();
    }
}
