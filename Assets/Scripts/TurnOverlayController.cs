using System.Collections;
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

public class TurnOverlayController : MonoBehaviour
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

    void UpdateGraph(List<double> valueList)
    {
        // Delete all points
        foreach (GameObject gameObject in gameObjectList)//problem
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        //Creat new graph
        //double xSize = graphWidth / maxAvailableVisible;
        //double xMax = valueList.Count;
        int xIndex = 0;
        GameObject lastPointGameObject = null;
        for (int i = Math.Max(valueList.Count - maxDotsVisible, 0); i < valueList.Count; i++)
        {
            //x = r cos theta. y = r sin theta. r is variable dependent of how old the data is. theta = turnDegrees.
            double r = radius / maxDotsVisible * (maxDotsVisible - xIndex); //maxDotsVisible or length of list?
            double degrees = valueList[xIndex];
            double xPos = r * Math.Cos(degrees);
            double yPos = r * Math.Sin(degrees);
            Debug.Log("dot info: " + degrees + ", " + r + ", " + xPos + ", " + yPos);
            GameObject pointGameObject = CreatePoint(xPos, yPos);
            gameObjectList.Add(pointGameObject);
            lastPointGameObject = pointGameObject;
            xIndex++;
        }
    }

        // Update is called once per frame
        void Update()
    {
        UpdateTurnValues();
        UpdateCompassOverlay();
        UpdateTextDisplay();
        _valueList.Add(turnDegrees);//problem
        List<double> turnsGraphPoints = SamplePoints(_valueList);
        //foreach (var x in _valueList)//shows growing list, not restricted list? for debugging (_valueList or turnsGraphPoints)
        //{
        //    Debug.Log(x.ToString());
        //}
        //Debug.Log(turnsGraphPoints);
        UpdateGraph(turnsGraphPoints);//problem
    }

    //Function to create each physical point on the graph
    private GameObject CreatePoint(double xPos, double yPos)
    {
        GameObject point = Instantiate(dot, new Vector3((float)xPos, (float)yPos, 0), Quaternion.identity, turnsGraphTransform); //turnsGraphTransform or turnsGraphContainer
        point.transform.parent = turnsGraphTransform;
        point.transform.localPosition = new Vector3((float)xPos, (float)yPos, 0);
        return point;
    }

    //Creates list of ppoints from livestream data
    private List<Double> SamplePoints(List<Double> points)
    {
        int indexIncr = points.Count / maxDotsVisible;
        List<Double> sampledPoints = new List<Double>();
        int multiplier = 0;
        while (sampledPoints.Count < Math.Min(maxDotsVisible, points.Count))
        {
            sampledPoints.Add(points[multiplier * indexIncr]);//having trouble with this one
            multiplier++;
        }
        return sampledPoints;
    }
}
