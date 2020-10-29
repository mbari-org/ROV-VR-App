using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class DepthOverlayController : MonoBehaviour
{
    // Listener
    public GameObject listenerGameObject;
    private LCMListener listener;

    // GameObject Reference
    public GameObject depthTextGameObject;
    public GameObject maxDepthGameObject;
    public GameObject depthGraphContainerGameObject;
    public GameObject dot;

    private RectTransform depthGraphContainer;
    private Transform depthGraphTransform;

    private List<GameObject> gameObjectList;

    

    private TextMeshProUGUI depth;
    private TextMeshProUGUI maxDepth;
    public List<double> _valueList;

  
    private double maxDepthDouble;
    private int maxAvailableVisible = 100;

    double graphHeight;
    double graphWidth;

    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        depth = depthTextGameObject.GetComponent<TextMeshProUGUI>();
        maxDepth = maxDepthGameObject.GetComponent<TextMeshProUGUI>();
        depthGraphContainer = depthGraphContainerGameObject.GetComponent<RectTransform>();
        depthGraphTransform = depthGraphContainerGameObject.GetComponent<Transform>();
        // Initialize list of points
        gameObjectList = new List<GameObject>();

        // Initialize Variables
        maxDepthDouble = 0;

        graphHeight = depthGraphContainer.sizeDelta.y;
        graphWidth = depthGraphContainer.sizeDelta.x;



    }

    // Update is called once per frame
    void Update()
    {

        // Break out of update if depth value is still at 0
        if (listener.Depth < 0.01)
        {
            return;
        }        
        // Update max depth
        if (listener.Depth > maxDepthDouble)
        {
            maxDepthDouble = listener.Depth;
        }
        _valueList.Add(listener.Depth);

        // Update depth labels
        depth.SetText("Depth: " + Math.Round(listener.Depth, 2).ToString());
        maxDepth.SetText(Math.Round(maxDepthDouble, 2).ToString() + " m");

        List<double> depthGraphPoints = SamplePoints(_valueList);
        DrawGraph(depthGraphPoints);

    }

    void DrawGraph(List<double> valueList)
    {
        // Delete all points
        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        

        double xSize = graphWidth / maxAvailableVisible;
        double xMax = valueList.Count;
        int xIndex = 0;
        GameObject lastPointGameObject = null;
        for (int i=Math.Max(valueList.Count - maxAvailableVisible, 0); i<valueList.Count; i++)
        {
            double xPos = xSize + xIndex * xSize;
            double yPos = graphHeight - (valueList[i] / maxDepthDouble) * graphHeight;
            GameObject pointGameObject = CreatePoint(xPos, yPos);
            gameObjectList.Add(pointGameObject);
            lastPointGameObject = pointGameObject;
            xIndex++;
        }
        

    }
    
    private GameObject CreatePoint(double xPos, double yPos)
    {
        GameObject point = Instantiate(dot, new Vector3((float) xPos, (float) yPos, 0), Quaternion.identity, depthGraphTransform);
        point.transform.parent = depthGraphTransform;
        point.transform.localPosition = new Vector3((float)xPos, (float)yPos, 0);
        return point;

    }

    private List<Double> SamplePoints(List<Double> points)
    {
        int indexIncr = points.Count / maxAvailableVisible;
        List<Double> sampledPoints = new List<Double>();
        int multiplier = 0;
        while (sampledPoints.Count < Math.Min(maxAvailableVisible, points.Count))
        {
            sampledPoints.Add(points[multiplier * indexIncr]);
            multiplier++;
        }
        return sampledPoints;
    }


}
