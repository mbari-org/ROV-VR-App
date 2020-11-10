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
    private double secondsElapsed;

    // storing fake data
    private double simulated_yaw;
    private double simulated_turns;

    // for publishing
    private string publisher_name = "FakeLCMPublisher";
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
        return degrees * Math.PI / 180.0;
    }

    double SecondsElapsedToAccumYaw(double seconds_elapsed, YawDataOptions yawDataOptions)
    {
        //// Convert the seconds to radians
        //double radians = seconds_elapsed * 180.0 / Math.PI;

        // Create a container for the output of the function
        double accumulated_yaw = 0.0;


        if (yawDataOptions == YawDataOptions.TwoFullRotationsLeft)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90)
            {
                // Yaw accumulates from 0 to 360 degrees from 0 to 90 seconds
                accumulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed));
            }
            else if (seconds_elapsed > 90 && seconds_elapsed <= 180)
            {
                // Yaw continues accumulating from 360 to 720 degrees from 90 to 180 seconds
                accumulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed + 270.0)) + 360.0;
            }
            else
            {
                // Yaw stops accumulating at 720 degrees
                accumulated_yaw = 720.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.TwoFullRotationsLeftFast)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90.0 / 5.0)
            {
                // Yaw accumulates from 0 to 360 degrees from 0 to 90/5 seconds
                accumulated_yaw = 360.0 * Math.Sin(5.0 * DegreesToRadians(seconds_elapsed));
            }
            else if (seconds_elapsed > 90.0 / 5 && seconds_elapsed <= 180.0 / 5)
            {
                // Yaw accumulates from 360 to 720 degrees from 90/5 to 180/5 seconds
                accumulated_yaw = 360.0 * Math.Sin(5.0 * DegreesToRadians(seconds_elapsed + 270.0)) + 360.0;
            }
            else
            {
                // Yaw stops accumulating at 720 degrees
                accumulated_yaw = 720.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.TwoFullRotationsRight)
        {
            // Input seconds into a piece-wise function
            if (seconds_elapsed >= 0 && seconds_elapsed <= 90)
            {
                // Yaw accumulates from 0 to -360 degrees from 0 to 90 seconds
                accumulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed + 90.0)) - 360.0;
            }
            else if (seconds_elapsed > 90 && seconds_elapsed <= 180)
            {
                // Yaw continues to accumulate from -360 to -720 from 90 to 180 seconds
                accumulated_yaw = 360.0 * Math.Sin(DegreesToRadians(seconds_elapsed)) - 720.0;
            }
            else
            {
                accumulated_yaw = -720.0;
            }
        }

        else if (yawDataOptions == YawDataOptions.WiggleLeftAndRight)
        {
            accumulated_yaw = 10.0 * Math.Sin(30.0 * DegreesToRadians(seconds_elapsed)) + 180.0;
        }

        return accumulated_yaw;
    }

    void UpdateFakeData()
    {
        // Update the time elapsed since the start of the program
        currentTime = DateTime.Now;
        TimeSpan ts = currentTime - startTime;
        secondsElapsed = ts.TotalSeconds;

        // Calculate accumulated yaw
        double accumulated_yaw = SecondsElapsedToAccumYaw(secondsElapsed, YawDataOptions.TwoFullRotationsLeftFast);

        // Calculate turns and current yaw
        simulated_yaw = accumulated_yaw % 360.0;
        simulated_turns = accumulated_yaw / 360.0;
    }

    void PublishFakeAttitude()
    {
        // Create the appropriate message type
        mwt.mini_rov_attitude_t mini_rov_attitude_t_msg = new mwt.mini_rov_attitude_t();

        // Populate that message

        // First create and populate a header
        mwt.header_t header = new mwt.header_t();
        header.publisher = publisher_name;

        mini_rov_attitude_t_msg.header = header;
        mini_rov_attitude_t_msg.roll_deg = 0.0;
        mini_rov_attitude_t_msg.pitch_deg = 0.0;
        mini_rov_attitude_t_msg.yaw_deg = simulated_yaw;

        // Publish that message
        System.String channel_name = "MINIROV_ATTITUDE";
        myLCM.Publish(channel_name, mini_rov_attitude_t_msg);
    }

    void PublishFakeTurns()
    {
        // Create the appropriate message type
        //mwt.mini_rov_attitude_t mini_rov_attitude_t_msg = new mwt.mini_rov_attitude_t();
        mwt.mini_rov_turns_t mini_rov_turns_t_msg = new mwt.mini_rov_turns_t();

        // Populate that message

        // First create and populate a header
        mwt.header_t header = new mwt.header_t();
        header.publisher = publisher_name;

        mini_rov_turns_t_msg.header = header;
        mini_rov_turns_t_msg.turns = simulated_turns;

        // Publish that message
        System.String channel_name = "MINIROV_TURNS";
        myLCM.Publish(channel_name, mini_rov_turns_t_msg);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFakeData();
        PublishFakeAttitude();
        PublishFakeTurns();
    }
}
