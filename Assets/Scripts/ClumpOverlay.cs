using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class ClumpOverlay : MonoBehaviour
{
    // Listener
    public GameObject listenerGameObject;
    private LCMListener listener;

    // GameObject References
    public GameObject clumpGameObject;
    public GameObject distanceTextGameObject;


    private Transform clumpPosition;
    private TextMeshProUGUI distanceText;

    // Constants
    private float canvasCircumference = 1620f;
    private float offsetAdjustment = 0f; //to make arrow line up with values better
    private float curvedGUIAdjustment = 15f;
    float EarthRadius = 6378.137f; // Radius of earth in KM

    private float clumpDistance;

    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        clumpPosition = clumpGameObject.GetComponent<RectTransform>();
        distanceText = distanceTextGameObject.GetComponent<TextMeshProUGUI>();
        // Start thread
        StartCoroutine(UpdateDistance());
    }



    // Update is called once per frame
    void Update()
    {
        // Get clump theta
        float clumpTheta = CalculateROVtoClumpAngle();
        // To remove jitter
        Vector3 targetPos = clumpPosition.position;
        targetPos.x = -1f * (float)(clumpTheta / 360f) * (canvasCircumference + curvedGUIAdjustment) + offsetAdjustment;
        var diff = Mathf.Abs(clumpPosition.position.x - targetPos.x);
        var step = diff < 20 ? .1f : diff;
        //Update clump position
        clumpPosition.position = Vector3.MoveTowards(clumpPosition.position, targetPos, step);

        // Update distance text
        distanceText.SetText(Math.Round(clumpDistance, 1).ToString() + " m");


    }

    float CalculateROVtoClumpAngle()
    {
        double deltaLat = listener.ClumpLat - listener.ROVLat;
        double deltaLon = listener.ClumpLon - listener.ROVLon;
        float theta = Mathf.Atan2((float)deltaLon, (float)deltaLat);
        return ((float)listener.Yaw) - (theta * Mathf.Rad2Deg);
    }

    IEnumerator UpdateDistance()
    {
        while (true)
        {
            // Haversine Formula to calculate great-circle distance
            float deltaLat = (float)(listener.ClumpLat * Mathf.PI / 180 - listener.ROVLat * Mathf.PI / 180);
            float deltaLon = (float)(listener.ClumpLon * Mathf.PI / 180 - listener.ROVLon * Mathf.PI / 180);
            float a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2) +
            Mathf.Cos((float)listener.ROVLat * Mathf.PI / 180) * Mathf.Cos((float)listener.ClumpLat * Mathf.PI / 180) *
            Mathf.Sin(deltaLon / 2) * Mathf.Sin(deltaLon / 2);
            float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            float d = EarthRadius * c;
            clumpDistance = d * 1000f;
            yield return null;
        }

    }
}
