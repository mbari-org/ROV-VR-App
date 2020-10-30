using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO.Ports;
using System.Security.AccessControl;
using System.Collections.Specialized;

// Useful References:
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

    void UpdateTurnValues()
    {
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
    }

    void UpdateCompassOverlay()
    {
        // Set the rotation of the compass so that it matches the current heading
        compassGameObject.transform.rotation = Quaternion.Euler(90.0f, (float) currentYaw, 0.0f);
    }

    void UpdateTextDisplay()
    {
        // Calculate turns
        turns = DegreesToTurns(currentDelta);

        // Convert turns to string rounded to hundreth place
        string turnString = turns.ToString("0.00");

        // Add a + at the beginning if turns is positive
        if (turns > 0) { turnString = "+" + turnString; }

        // Display new turnString
        turnText.SetText(turnString);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurnValues();
        UpdateCompassOverlay();
        UpdateTextDisplay();
    }
}
