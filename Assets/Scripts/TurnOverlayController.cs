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

    // Initialize turns
    private double turns = 0.0;

    // Initialize turn degrees for compass
    private double turnDegrees = 0.0;

    // Start is called before the first frame update
    void Start()
    {
        // Set up LCM listener
        listener = listenerGameObject.GetComponent<LCMListener>();
        
        // Set up text display for turn
        turnText = turnLabelGameObject.GetComponent<TextMeshProUGUI>();
    }

    void UpdateTurnValues()
    {
        // Update turns based on listener's turns
        turns = listener.Turns;

        // Update turn degrees for rotating compass
        turnDegrees = (turns % 1.0) * 360.0;
    }

    void UpdateCompassOverlay()
    {
        // Set the rotation of the compass so that it matches the current turns
        compassGameObject.transform.rotation = Quaternion.Euler(90.0f, (float) turnDegrees, 0.0f);
    }

    void UpdateTextDisplay()
    {
        // Convert turns to string rounded to hundreth place
        string turnString = turns.ToString("0.00");

        // Add a + at the beginning if turns are positive
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
