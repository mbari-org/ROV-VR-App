using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO.Ports;
using System.Security.AccessControl;
using System.Collections.Specialized;

// Useful References for Ever:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html
// - https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html (for arrows)

public class TurnOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject compassGameObject;
    public GameObject turnLabelGameObject;

    private TextMeshProUGUI turnText;

    // heading relative from boat to ROV
    // Currently hardcoded, but must be set automatically in final release
    private double initYaw = 0.0;

    // Initialize yaw values
    private double previousYaw;
    private double currentYaw;
    private double previousDelta = 0.0;
    private double currentDelta = 0.0;
    private double rotationYaw = 0.0;
    private double turnOffset = 0.0;

    // Initialize turns
    private double turns = 0.0;

    // Debugging variables
    private int numUpdates = 0;

    // reminder: Heading goes from 0->360 (double)

    // Start is called before the first frame update
    void Start()
    {
        // Set up LCM listener
        listener = listenerGameObject.GetComponent<LCMListener>();
        
        // Set up text display for turn
        turnText = turnLabelGameObject.GetComponent<TextMeshProUGUI>();

        // Set up yaw based on initYaw
        currentYaw = initYaw;
    }

    double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    double DegreesToTurns(double degrees)
    {
        return degrees / 360.0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (numUpdates == 0)
        //{
        // Debug.Log("Yaw: " + listener.Yaw.ToString());

        // Update Yaw and Delta values
        previousYaw = currentYaw;
        currentYaw = listener.Yaw;
        previousDelta = currentDelta;
        currentDelta = currentYaw - initYaw + turnOffset;

        // Update the offset based on turns if necessary
        if (currentYaw <= 90.0 && previousYaw >= 270.0)
        {
            turnOffset = turnOffset + 360.0;
        }
        else if (currentYaw >= 270.0 && previousYaw <= 90.0)
        {
            turnOffset = turnOffset - 360.0;
        }

        // Rotate the compass relative to its last position
        rotationYaw = currentDelta - previousDelta;
        compassGameObject.transform.Rotate(0.0f, (float)rotationYaw, 0.0f, Space.World);

        // Calculate turns
        turns = DegreesToTurns(currentDelta);

        UnityEngine.Debug.Log(turns);
        //UnityEngine.Debug.Log(previousDelta);
        //UnityEngine.Debug.Log(currentDelta);
        //UnityEngine.Debug.Log(rotationYaw);

        // NOTE: Rotation is relative to where the object is at that point in time

        // Split string into before decimal and after decimal
        // make it so that the latter has one decimal point of precision
        // combine strings back together
        // display final string in TurnText

        string yawString = turns.ToString();
        
        // TODO: Make this round to the nearest decimal place instead of just cutting off past a certain decmial
        // Stop here if the yawValue was never set properly
        string[] yawSubStrings = yawString.Split('.');
        if (yawSubStrings.Length != 2) { return; }

        // Cut off certain decimal places
        yawSubStrings[1] = yawSubStrings[1].Substring(0, 2);

        // Reconnect substrings into big string
        string turnString = yawSubStrings[0] + "." + yawSubStrings[1];

        turnText.SetText(turnString);
        //}
        //numUpdates = numUpdates + 1;
    }
}
