using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DataCanvas : MonoBehaviour
{
    public LCMListener listener;
    public Text deltaText;
    public Text depthText;
    public Text rollText;
    public Text pitchText;
    public Text yawText;
    public Text rovLatText;
    public Text rovLonText;
    public Text shipLatText;
    public Text shipLonText;
    public Text clumpLatText;
    public Text clumpLonText;

    public Text debugText;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        double deltaValue = Math.Round(listener.ClumpDelta, 3);
        deltaText.text = "Clump Delta: " + deltaValue.ToString();

        double depthValue = Math.Round(listener.Depth, 3);
        depthText.text = "Depth: " + depthValue.ToString();

        double rollValue = Math.Round(listener.Roll, 3);
        rollText.text = "Roll: " + rollValue.ToString();

        double pitchValue = Math.Round(listener.Pitch, 3);
        pitchText.text = "Pitch: " + pitchValue.ToString();

        double yawValue = Math.Round(listener.Yaw, 3);
        yawText.text = "Yaw: " + yawValue.ToString();

        double rovLatValue = Math.Round(listener.ROVLat, 3);
        rovLatText.text = "ROV Lat: " + rovLatValue.ToString();

        double rovLonValue = Math.Round(listener.ROVLon, 3);
        rovLonText.text = "ROV Lon: " + rovLonValue.ToString();

        double shipLatValue = Math.Round(listener.ShipLat, 3);
        shipLatText.text = "Ship Lat: " + shipLatValue.ToString();

        double shipLonValue = Math.Round(listener.ShipLon, 3);
        shipLonText.text = "Ship Lon: " + shipLonValue.ToString();

        double clumpLatValue = Math.Round(listener.ClumpLat, 3);
        clumpLatText.text = "Clump Lat: " + clumpLatValue.ToString();

        double clumpLonValue = Math.Round(listener.ClumpLon, 3);
        clumpLonText.text = "Clump Lon: " + clumpLonValue.ToString();
    }

    public void printDebug(String msg)
    {
        debugText.text = debugText.text + "\n" + msg;
    }
}
