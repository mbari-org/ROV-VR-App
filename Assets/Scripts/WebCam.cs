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

    // Will be used to overlay raw image on top of default background
    private Texture defaultBackground1;
    private Texture defaultBackground2;
    private Texture defaultBackground3;
    private Texture defaultBackground4;
    private Texture defaultBackground5;
    private Texture[] defaultBackgroundArray = new Texture[maxCameras];

    // Gameobjects that need to be set in inspector menu
    public RawImage background1;
    public RawImage background2;
    public RawImage background3;
    public RawImage background4;
    public RawImage background5;
    private RawImage[] backgroundArray = new RawImage[maxCameras];

    public AspectRatioFitter fit1;
    public AspectRatioFitter fit2;
    public AspectRatioFitter fit3;
    public AspectRatioFitter fit4;
    public AspectRatioFitter fit5;
    private AspectRatioFitter[] aspectRatioArray = new AspectRatioFitter[maxCameras];

    void Start()
    {
        // TODO: Put this in a loop
        defaultBackgroundArray[0] = defaultBackground1;
        defaultBackgroundArray[1] = defaultBackground2;
        defaultBackgroundArray[2] = defaultBackground3;
        defaultBackgroundArray[3] = defaultBackground4;
        defaultBackgroundArray[4] = defaultBackground5;

        backgroundArray[0] = background1;
        backgroundArray[1] = background2;
        backgroundArray[2] = background3;
        backgroundArray[3] = background4;
        backgroundArray[4] = background5;

        aspectRatioArray[0] = fit1;
        aspectRatioArray[1] = fit2;
        aspectRatioArray[2] = fit3;
        aspectRatioArray[3] = fit4;
        aspectRatioArray[4] = fit5;

        // Set background as our image texture
        for (int i = 0; i < maxCameras; i++)
            defaultBackgroundArray[i] = backgroundArray[i].texture;

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
            //if (devices[i].name != "Leap Dev Kit")

            // Make sure it's a Blackmagic camera
            if (devices[i].name.Contains("Black"))
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
            float ratio = (float)camArray[i].width / (float)camArray[i].height;
            fit1.aspectRatio = ratio;
            float scaleY = camArray[i].videoVerticallyMirrored ? -1f : 1f;
            backgroundArray[i].rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            int orient = -camArray[i].videoRotationAngle;
            backgroundArray[i].rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }
}