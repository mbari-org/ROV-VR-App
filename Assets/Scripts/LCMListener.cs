using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LCM;
using LCM.LCM;
using UnityEngine;
using UnityEngine.UI;

public class LCMListener : MonoBehaviour
{   
    private static LCMListener instance;
    SimpleSubscriber mySubscriber;

    // mini_rov_attitude_t
    private double roll_deg;
    public double Roll {get {return roll_deg;}}

    private double pitch_deg;
    public double Pitch {get {return pitch_deg;}}

    private double yaw_deg;
    public double Yaw {get {return yaw_deg;}}

    // mini_rov_turns_t
    private double turns;
    public double Turns {get {return turns;}}

    // mini_rov_depth_t
    private double depth;
    public double Depth {get {return depth;}}

    private double pressure;
    public double Pressure {get {return pressure;}}

    private double rope_length;
    public double RopeLength {get {return rope_length;}}

    private double clump_delta;
    public double ClumpDelta {get {return clump_delta;}}

    private double rov_lon;
    public double ROVLon {get {return rov_lon;}}

    private double rov_lat;
    public double ROVLat {get {return rov_lat;}} 

    private double ship_lon;
    public double ShipLon {get {return ship_lon;}}

    private double ship_lat;
    public double ShipLat {get {return ship_lat;}}
    private double clump_lon;
    public double ClumpLon { get { return clump_lon; } }

    private double clump_lat;
    public double ClumpLat { get { return clump_lat; } }

    LCM.LCM.LCM myLCM;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        setURL("udpm://239.255.76.67:7667"); // Default LCM URL

        StartCoroutine(SimulateRopeLength(3, 4, .01d));
    }

    class SimpleSubscriber : LCM.LCM.LCMSubscriber
    {
        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
        {
            if (channel == "MINIROV_ATTITUDE")
            {
                mwt.mini_rov_attitude_t msg = new mwt.mini_rov_attitude_t(dins);
                instance.roll_deg = msg.roll_deg;
                instance.pitch_deg = msg.pitch_deg;
                instance.yaw_deg = msg.yaw_deg;

            }
            else if (channel == "MINIROV_TURNS")
            {
                mwt.turns_t msg = new mwt.turns_t(dins);
                instance.turns = msg.turns;
            }
            else if (channel == "MINIROV_DEPTH")
            {
                mwt.mini_rov_depth_t msg = new mwt.mini_rov_depth_t(dins);
                instance.depth = msg.depth;
                instance.pressure = msg.pressure;
            }
            else if (channel == "MWT_STEREO_IMAGE")
            {
                mwt.stereo_image_t msg = new mwt.stereo_image_t(dins);
            }
            else if (channel == "MINIROV_ROWE_DVL")
            {
                mwt.mini_rov_rowe_dvl_t msg = new mwt.mini_rov_rowe_dvl_t(dins);
            }
            else if (channel == "SHIP_POSITION")
            {
                mwt.position_t msg = new mwt.position_t(dins);
                instance.ship_lat = msg.latitude_deg;
                instance.ship_lon = msg.longitude_deg;
            }
            else if (channel == "MINIROV_POSITION")
            {
                mwt.position_t msg = new mwt.position_t(dins);
                instance.rov_lat = msg.latitude_deg;
                instance.rov_lon = msg.longitude_deg;
            }
            else if (channel == "CLUMP_POSITION")
            {
                mwt.position_t msg = new mwt.position_t(dins);
                instance.clump_lat = msg.latitude_deg;
                instance.clump_lon = msg.longitude_deg;       
            }
            else if (channel == "CLUMP_STATUS")
            {
                mwt.clump_status_t msg = new mwt.clump_status_t(dins);
                instance.clump_delta = msg.delta_m;
            }
            else
            {
                // Debug.Log("Unknown Channel: " + channel);
            }
        }
    }

    IEnumerator SimulateRopeLength(double min, double max, double step) 
    {   while (true) {
            for (double f = min; f <  max; f += step) 
            {
                rope_length = f;
                yield return new WaitForSeconds(.2f);
            }
            for (double f = max; f > min; f -= step) 
            {
                rope_length = f;
                yield return new WaitForSeconds(.2f);
            }
        }
    }

    public void changeURL(string newURL)
    {
        // Close connection to previous URL
        myLCM.Close();

        // Set new URL
        setURL(newURL);
    }

    void setURL(string newURL)
    {
        print("Attempting to use LCM URL: " + newURL);
        try
        {
            // Create new LCM object
            myLCM = new LCM.LCM.LCM(newURL);
        }
        catch (Exception)
        {
            print("Invalid or Unset LCM URL - reverting to default");
            myLCM = new LCM.LCM.LCM("udpm://239.255.76.67:7667");
        }

        mySubscriber = new SimpleSubscriber();
        myLCM.SubscribeAll(mySubscriber);
    }


    // Update is called once per frame
    void Update()
    {
    }
}
