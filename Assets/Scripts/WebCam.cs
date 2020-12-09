using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WebCam : MonoBehaviour
{
    public static int maxCameras = 5;

    private bool camAvailable;
    private int numCameras = 0;

    private WebCamTexture[] camArray = new WebCamTexture[maxCameras];

    // Gameobjects that need to be set in inspector menu
    public GameObject display1;
    public GameObject display2;
    public GameObject display3;
    public GameObject display4;
    public GameObject display5;

    private RawImage[] backgroundArray = new RawImage[maxCameras];
    private AspectRatioFitter[] aspectRatioArray = new AspectRatioFitter[maxCameras];

    void Start()
    {
        // TODO: Put this in a loop
        backgroundArray[0] = display1.GetComponentInChildren(typeof(RawImage)) as RawImage;
        backgroundArray[1] = display2.GetComponentInChildren(typeof(RawImage)) as RawImage;
        backgroundArray[2] = display3.GetComponentInChildren(typeof(RawImage)) as RawImage;
        backgroundArray[3] = display4.GetComponentInChildren(typeof(RawImage)) as RawImage;
        backgroundArray[4] = display5.GetComponentInChildren(typeof(RawImage)) as RawImage;

        aspectRatioArray[0] = display1.GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
        aspectRatioArray[1] = display2.GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
        aspectRatioArray[2] = display3.GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
        aspectRatioArray[3] = display4.GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
        aspectRatioArray[4] = display5.GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;

        // Find Webcams
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("no camera detected");
            camAvailable = false;
            return;
        }

        // Initialize all devices
        int currDisplayIdx = 0;
        for (int i = 0; i < devices.Length; i++)
        {
            // Make sure it's not a leapmotion camera
            if (devices[i].name != "Leap Dev Kit")

            // Make sure it's a Blackmagic camera
            //if (devices[i].name.Contains("Black"))
            {
                Debug.Log(i);
                Debug.Log(devices[i].name);

                // Initialize camera
                camArray[currDisplayIdx] = new WebCamTexture(devices[i].name, Screen.width, Screen.height);

                // Play camera if not already playing
                if (!camArray[currDisplayIdx].isPlaying)
                    camArray[currDisplayIdx].Play();

                // Display camera feed
                backgroundArray[currDisplayIdx].texture = camArray[currDisplayIdx];

                currDisplayIdx++;
                numCameras++;
            }
        }
        camAvailable = true;
    }
    void Update()
    {
        if (!camAvailable)
            return;

        for (int i = 0; i< numCameras; i++)
        {
            //float ratio = (float)camArray[i].width / (float)camArray[i].height;
            //fit1.aspectRatio = ratio;
            float scaleY = camArray[i].videoVerticallyMirrored ? -1f : 1f;
            backgroundArray[i].rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            int orient = -camArray[i].videoRotationAngle;
            backgroundArray[i].rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }
}