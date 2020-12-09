using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO: Disable unused displays
// TODO: Release camera when deselected
// TODO: Allow for duplicates (check before playing)
// TODO: Figure out if aspect ratio fitter is necessary
// TODO: create list of "webcam textures" - one for each camera (and check to make sure they don't break things)

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

    private GameObject[] displayArray = new GameObject[maxCameras];
    private RawImage[] backgroundArray = new RawImage[maxCameras];
    private AspectRatioFitter[] aspectRatioArray = new AspectRatioFitter[maxCameras];
    private Dropdown[] dropdownArray = new Dropdown[maxCameras];

    // For dropdown menu
    List<string> cameraNameList = new List<string>();

    void Start()
    {
        displayArray[0] = display1;
        displayArray[1] = display2;
        displayArray[2] = display3;
        displayArray[3] = display4;
        displayArray[4] = display5;

        for (int i = 0; i < maxCameras; i++)
        {
            backgroundArray[i] = displayArray[i].GetComponentInChildren(typeof(RawImage)) as RawImage;
            aspectRatioArray[i] = displayArray[i].GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
            dropdownArray[i] = displayArray[i].GetComponentInChildren(typeof(Dropdown)) as Dropdown;

            Debug.Log(aspectRatioArray[i]);
            Debug.Log(dropdownArray[i]);
        }

        // Find Webcams
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("no camera detected");
            camAvailable = false;
            return;
        }

        // Update list of device names
        cameraNameList.Add("[Select Camera]");
        for (int i = 0; i < devices.Length; i++)
            cameraNameList.Add((string)devices[i].name);
         
        // Update dropdown options
        for (int i = 0; i < maxCameras; i++)
        {   
            dropdownArray[i].ClearOptions();
            dropdownArray[i].AddOptions(cameraNameList);
        }
        // TODO: Insert this in the above loop without things breaking
        dropdownArray[0].onValueChanged.AddListener(delegate { InitializeCamera(dropdownArray[0], (int)0, devices); });
        dropdownArray[1].onValueChanged.AddListener(delegate { InitializeCamera(dropdownArray[1], (int)1, devices); });
        dropdownArray[2].onValueChanged.AddListener(delegate { InitializeCamera(dropdownArray[2], (int)2, devices); });
        dropdownArray[3].onValueChanged.AddListener(delegate { InitializeCamera(dropdownArray[3], (int)3, devices); });
        dropdownArray[4].onValueChanged.AddListener(delegate { InitializeCamera(dropdownArray[4], (int)4, devices); });

        // Initialize all devices
        //int currDisplayIdx = 0;
        //for (int i = 0; i < devices.Length; i++)
        //{
        //    // Make sure it's not a leapmotion camera
        //    if (devices[i].name != "Leap Dev Kit")

        //    // Make sure it's a Blackmagic camera
        //    //if (devices[i].name.Contains("Black"))
        //    {
        //        Debug.Log(i);
        //        Debug.Log(devices[i].name);

        //        // Initialize camera
        //        camArray[currDisplayIdx] = new WebCamTexture(devices[i].name, Screen.width, Screen.height);

        //        // Play camera if not already playing
        //        if (!camArray[currDisplayIdx].isPlaying)
        //            camArray[currDisplayIdx].Play();

        //        // Display camera feed
        //        backgroundArray[currDisplayIdx].texture = camArray[currDisplayIdx];

        //        currDisplayIdx++;
        //        numCameras++;
        //    }
        //}
        //camAvailable = true;
    }

    //Ouput the new value of the Dropdown into Text
    void InitializeCamera(Dropdown dropdown, int displayIdx, WebCamDevice[] devices)
    {
        // Ignore [Select Camera] option
        if (dropdown.value != 0)
        {
            int cameraIdx = dropdown.value - 1; // Skip [Select Camera] option
            
            Debug.Log("Initializing : " + dropdown.value);
            // Initialize camera
            camArray[displayIdx] = new WebCamTexture(devices[cameraIdx].name, Screen.width, Screen.height);

            // Play camera if not already playing
            if (!camArray[displayIdx].isPlaying)
                camArray[displayIdx].Play();

            // Display camera feed
            backgroundArray[displayIdx].texture = camArray[displayIdx];

        }
    }
    void Update()
    {
        //if (!camAvailable)
        //    return;

        for (int i = 0; i < numCameras; i++)
        {
            // Only update initialized cameras
            if (camArray[i] != null)
            {
                float ratio = (float)camArray[i].width / (float)camArray[i].height;
                aspectRatioArray[i].aspectRatio = ratio;
                float scaleY = camArray[i].videoVerticallyMirrored ? -1f : 1f;
                backgroundArray[i].rectTransform.localScale = new Vector3(1f, scaleY, 1f);
                int orient = -camArray[i].videoRotationAngle;
                backgroundArray[i].rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            }
        }
    }
}