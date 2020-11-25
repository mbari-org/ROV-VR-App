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
    public GameObject recentDepthTextGameObject;
    public GameObject recentMaxDepthGameObject;
    public GameObject recentDepthGraphContainerGameObject;
    public GameObject dot;

    private RectTransform depthGraphContainer;
    private Transform depthGraphTransform;
    private RectTransform recentDepthGraphContainer;
    private Transform recentDepthGraphTransform;

    private List<GameObject> gameObjectList;

    

    private TextMeshProUGUI depth;
    private TextMeshProUGUI maxDepth;
    private TextMeshProUGUI recentDepth;
    private TextMeshProUGUI recentMaxDepth;
    public List<double> _valueList;

  
    private double maxDepthDouble;
    private int maxAvailableVisible = 100;

    double graphHeight;
    double graphWidth;
    double recentGraphWidth;

    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        depth = depthTextGameObject.GetComponent<TextMeshProUGUI>();
        maxDepth = maxDepthGameObject.GetComponent<TextMeshProUGUI>();
        recentDepth = recentDepthTextGameObject.GetComponent<TextMeshProUGUI>();
        recentMaxDepth = recentMaxDepthGameObject.GetComponent<TextMeshProUGUI>();
        depthGraphContainer = depthGraphContainerGameObject.GetComponent<RectTransform>();
        depthGraphTransform = depthGraphContainerGameObject.GetComponent<Transform>();
        recentDepthGraphContainer = recentDepthGraphContainerGameObject.GetComponent<RectTransform>();
        recentDepthGraphTransform = recentDepthGraphContainerGameObject.GetComponent<Transform>();
        // Initialize list of points
        gameObjectList = new List<GameObject>();

        // Initialize Variables
        maxDepthDouble = 0;

        graphHeight = depthGraphContainer.sizeDelta.y;
        graphWidth = depthGraphContainer.sizeDelta.x;
        recentGraphWidth = recentDepthGraphContainer.sizeDelta.x;



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
        List<double> recentDepthGraphPoints = SampleRecentPoints(_valueList, 100);
        DrawGraph(depthGraphPoints, depthGraphTransform, true, graphWidth);
        DrawGraph(recentDepthGraphPoints, recentDepthGraphTransform, false, recentGraphWidth);

    }

    void DrawGraph(List<double> valueList, Transform parentContainerTransform, bool clearGraph, double graphMaxWidth)
    {
        // Delete all points
        if (clearGraph)
        {
            foreach (GameObject gameObject in gameObjectList)
            {
                Destroy(gameObject);
            }
            gameObjectList.Clear();
        }
        
        double xSize = graphMaxWidth / maxAvailableVisible;
        double xMax = valueList.Count;
        int xIndex = 0;
        GameObject lastPointGameObject = null;
        for (int i=Math.Max(valueList.Count - maxAvailableVisible, 0); i<valueList.Count; i++)
        {
            double xPos = xSize + xIndex * xSize;
            double yPos = graphHeight - (valueList[i] / maxDepthDouble) * graphHeight;
            GameObject pointGameObject = CreatePoint(xPos, yPos, parentContainerTransform);
            gameObjectList.Add(pointGameObject);
            lastPointGameObject = pointGameObject;
            xIndex++;
        }
        

    }
    
    private GameObject CreatePoint(double xPos, double yPos, Transform parentContainerTransform)
    {
        GameObject point = Instantiate(dot, new Vector3((float) xPos, (float) yPos, 0), Quaternion.identity, parentContainerTransform);
        point.transform.parent = parentContainerTransform;
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

    private List<Double> SampleRecentPoints(List<Double> points, int recentCount)
    {
        List<Double> sampledPoints = new List<Double>();
        for (int i = Math.Max(0, points.Count - recentCount); i < points.Count; ++i)
        {
            sampledPoints.Add(points[i]);
        }
        return sampledPoints;
    }


}
