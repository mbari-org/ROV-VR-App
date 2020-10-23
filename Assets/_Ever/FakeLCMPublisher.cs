using System.Collections;
using System.Collections.Generic;
using LCM;
using LCM.LCM;
using UnityEngine;
using System.Diagnostics;
using System;

// Assumptions / Questions
// In LCM, Yaw goes from 0->360
// Positive value is turning left
// Negative value is turning right
// Yaw from LCM is relative to a global frame? Or possibly relative to boat? In which case, that _is_ the delta

public class FakeLCMPublisher : MonoBehaviour
{
    // for keeping track of this class
    private static FakeLCMPublisher instance;

    // for tracking time
    private DateTime startTime;
    private DateTime currentTime;

    // for publishing
    LCM.LCM.LCM myLCM;

    private enum YawDataOptions
    {
        TwoFullRotationsLeft,
        TwoFullRotationsLeftFast,
        TwoFullRotationsRight,
        WiggleLeftAndRight,
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        myLCM = new LCM.LCM.LCM();
        startTime = DateTime.Now;
    }
    
    double DegreesToRadians(double degrees)
    {
        return degrees *  Math.PI / 180.0;
    }

    double SecondsElapsedToYaw(double seconds_elapsed, YawDataOptions yawDataOptions)
    {
        //// Convert the seconds to radians
        //double radians = seconds_elapsed * 180.0 / Math.PI;

        // Create a container for the output of the function
        double simulated_yaw = 0.0;


        if (yawDataOptions == YawDataOptions.TwoFullRotationsLeft)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90)
            {
                simulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed));
            }
            else if (seconds_elapsed > 90 && seconds_elapsed <= 180)
            {
                simulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed + 270.0));
            }
            else
            {
                simulated_yaw = 360.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.TwoFullRotationsLeftFast)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90.0/5.0)
            {
                simulated_yaw = 360.0 * Math.Sin(5.0 * DegreesToRadians(seconds_elapsed));
            }
            else if (seconds_elapsed > 90.0/5 && seconds_elapsed <= 180.0/5)
            {
                simulated_yaw = 360.0 * Math.Sin(5.0 * DegreesToRadians(seconds_elapsed + 270.0));
            }
            else
            {
                simulated_yaw = 360.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.TwoFullRotationsRight)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90)
            {
                simulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed + 90.0));
            }
            else if (seconds_elapsed > 90 && seconds_elapsed <= 180)
            {
                simulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed));
            }
            else
            {
                simulated_yaw = 360.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.WiggleLeftAndRight)
        {
            simulated_yaw = 10.0 * Math.Sin(30.0 * DegreesToRadians(seconds_elapsed)) + 180.0;
        }

        return simulated_yaw;
    }

    void PublishFakeAttitude()
    {
        // Create the appropriate message type
        mwt.mini_rov_attitude_t mini_rov_attitude_t_msg = new mwt.mini_rov_attitude_t();

        // Create fake data dependent on the time

        // get the time elapsed since the beginning of the program
        currentTime = DateTime.Now;
        TimeSpan ts = currentTime - startTime;
        double secondsElapsed = ts.TotalSeconds;

        // Map that time elapsed to a yaw angle
        double simulatedYaw = SecondsElapsedToYaw(secondsElapsed, YawDataOptions.TwoFullRotationsLeftFast);

        // UnityEngine.Debug.Log(ts.Milliseconds);

        // Populate that message

        // First create and populate a header
        mwt.header_t header = new mwt.header_t();
        header.publisher = "Ever's Fake Publisher";

        mini_rov_attitude_t_msg.header = header;
        mini_rov_attitude_t_msg.roll_deg = 0.0;
        mini_rov_attitude_t_msg.pitch_deg = 0.0;
        mini_rov_attitude_t_msg.yaw_deg = simulatedYaw;

        // Publish that message
        System.String channel_name = "MINIROV_ATTITUDE";

        myLCM.Publish(channel_name, mini_rov_attitude_t_msg);
    }

    // Update is called once per frame
    void Update()
    {
        PublishFakeAttitude();
    }
}
