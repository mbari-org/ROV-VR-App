﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//why?
using TMPro;
using System;
using System.Net;//why?
using System.IO.Ports;
using System.Security.AccessControl;
using System.Security.Cryptography;//why?
using System.Collections.Specialized;
using System.Runtime.Remoting.Metadata.W3cXsd2001;//why?

// Useful References:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html
// - https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html (for arrows)

public class TurnsTake2 : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject compassGameObject;
    public GameObject turnLabelGameObject;

    private TextMeshProUGUI turnText;

    //Pertaining to historical data graphing
    public GameObject turnsGraphContainerGameObject;
    private RectTransform turnsGraphContainer;
    private Transform turnsGraphTransform;
    private List<GameObject> gameObjectList; //list of dots
    public GameObject dot;
    private int maxDotsVisible = 100; //max allowable # of historical turn data points

    public List<double> _valueList;

    // Initialize turns
    private double turns = 0.0;

    // Initialize turn degrees for compass
    private double turnDegrees = 0.0;
    private double turnDegreesOffset = 0.0;

    // Initialize turn radians for compass
    private double radians = 0.0;

    // Radius of turns overlay circle (approx)
    private double radius = 12.0;


    // Start is called before the first frame update
    void Start()
    {
        // Set up LCM listener
        listener = listenerGameObject.GetComponent<LCMListener>();

        // Set up text display for turn
        turnText = turnLabelGameObject.GetComponent<TextMeshProUGUI>();

        //Set up container for graph points
        turnsGraphContainer = turnsGraphContainerGameObject.GetComponent<RectTransform>();
        turnsGraphTransform = turnsGraphContainerGameObject.GetComponent<Transform>();

        // Initialize list of points
        gameObjectList = new List<GameObject>();

        //Test point
        //CreatePoint(-8, -8);
    }

    void UpdateTurnValues()
    {
        // Update turns based on listener's turns
        turns = listener.Turns;

        // Update turn degrees for rotating compass
        turnDegrees = (turns % 1.0) * 360.0;

        //degrees to radians (+ offset by 90 degrees and reverse turn)
        turnDegreesOffset = turnDegrees * -1.0 + 90.0;
        radians = turnDegreesOffset * Mathf.Deg2Rad;
    }

    void UpdateCompassOverlay()
    {
        // Set the rotation of the compass so that it matches the current turns
        compassGameObject.transform.rotation = Quaternion.Euler(90.0f, (float)turnDegrees, 0.0f);
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

    void UpdateGraph()
    {
        //x = r cos theta. y = r sin theta. r is variable dependent of how old the data is. theta = turnDegrees.
        double r = radius;
        double xPos = r * Math.Cos(radians);
        double yPos = r * Math.Sin(radians);
        Debug.Log(turnDegrees + ", " + xPos + ", " + yPos);
        GameObject pointGameObject = CreatePoint(xPos, yPos);

        // Delete all points
        //Destroy(pointGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurnValues();
        UpdateCompassOverlay();
        UpdateTextDisplay();
        UpdateGraph();//problem
    }

    //Function to create each physical point on the graph
    private GameObject CreatePoint(double xPos, double yPos)
    {
        GameObject point = Instantiate(dot, new Vector3((float)xPos, (float)yPos, 0), Quaternion.identity, turnsGraphTransform); //turnsGraphTransform or turnsGraphContainer
        point.transform.parent = turnsGraphTransform;
        point.transform.localPosition = new Vector3((float)xPos, (float)yPos, 0);
        return point;
    }
}
