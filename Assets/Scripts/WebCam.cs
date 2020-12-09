using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO: Disable unused displays
// TODO: Release unused cameras (having them all run might affect runtime?)
// TODO: Figure out if aspect ratio fitter is necessary
// TODO: enable hotplugging

// Note: Some cameras will throw a "could not start graph" and "could not pause pControl" error - 
// these will just not be displayed, but this could be handled more elegantly
// Note: all webcams need to be plugged in in advance

public class WebCam : MonoBehaviour
{
    public static int maxCameras = 5;

    private bool camAvailable;
    private int numCameras = 0;
    private List<WebCamTexture> camList = new List<WebCamTexture>();

    // Gameobjects that need to be set in inspector menu
    public Dropdown skyBoxDropdown;
    public Material skyBoxMaterial;
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
        }

        // Find Webcams
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("no camera detected");
            camAvailable = false;
            return;
        }

        // Update list of devices
        cameraNameList.Add("[Select Camera]");
        for (int i = 0; i < devices.Length; i++)
        {
            // Save camera name
            cameraNameList.Add((string)devices[i].name);

            // Create texture for each camera
            camList.Add(new WebCamTexture(devices[i].name, Screen.width, Screen.height));

            // Play camera if not already playing
            if (!camList[i].isPlaying)
                camList[i].Play();
        }

        // Update dropdown options
        skyBoxDropdown.ClearOptions();
        skyBoxDropdown.AddOptions(cameraNameList);
        for (int i = 0; i < maxCameras; i++)
        {
            dropdownArray[i].ClearOptions();
            dropdownArray[i].AddOptions(cameraNameList);
        }
        // TODO: Insert this in the above loop without things breaking
        skyBoxDropdown.onValueChanged.AddListener(delegate { DisplayCamera(skyBoxDropdown, (int)-1, devices); });
        dropdownArray[0].onValueChanged.AddListener(delegate { DisplayCamera(dropdownArray[0], (int)0, devices); });
        dropdownArray[1].onValueChanged.AddListener(delegate { DisplayCamera(dropdownArray[1], (int)1, devices); });
        dropdownArray[2].onValueChanged.AddListener(delegate { DisplayCamera(dropdownArray[2], (int)2, devices); });
        dropdownArray[3].onValueChanged.AddListener(delegate { DisplayCamera(dropdownArray[3], (int)3, devices); });
        dropdownArray[4].onValueChanged.AddListener(delegate { DisplayCamera(dropdownArray[4], (int)4, devices); });

        camAvailable = true;
    }

    void DisplayCamera(Dropdown dropdown, int displayIdx, WebCamDevice[] devices)
    {
        // Reset display if [Select Camera] is chosen
        if (dropdown.value == 0)
        {
            if (displayIdx == -1) // Skybox camera feed
                skyBoxMaterial.mainTexture = null;
            else // Non-skybox camera feed
                backgroundArray[displayIdx].texture = null;
        } 
        
        // Update camera display if a camera is chosen
        else
        {
            int cameraIdx = dropdown.value - 1; // Skip [Select Camera] option

            if (displayIdx == -1) // Skybox camera feed
                skyBoxMaterial.mainTexture = camList[cameraIdx];
            else // Non-skybox camera feed
                backgroundArray[displayIdx].texture = camList[cameraIdx];
        }
    }
    void Update()
    {
        if (!camAvailable)
            return;

        for (int i = 0; i < numCameras; i++)
        {
            // Only update initialized cameras
            if (camList[i] != null)
            {
                float ratio = (float)camList[i].width / (float)camList[i].height;
                aspectRatioArray[i].aspectRatio = ratio;
                float scaleY = camList[i].videoVerticallyMirrored ? -1f : 1f;
                backgroundArray[i].rectTransform.localScale = new Vector3(1f, scaleY, 1f);
                int orient = -camList[i].videoRotationAngle;
                backgroundArray[i].rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            }
        }
    }
}