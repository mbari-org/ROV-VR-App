using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // For checking if items exist in array

// TODO: Disable unused displays
// TODO: Figure out if aspect ratio fitter is necessary
// TODO: enable hotplugging
// TODO: Create a button that makes new displays

// Note: Some cameras will throw a "could not start graph" and "could not pause pControl" error - 
// these will just not be displayed, but this could be handled more elegantly
// Note: all webcams need to be plugged in in advance

public class WebCam : MonoBehaviour
{
    public static int numDisplays = 5;

    private bool camAvailable;
    // List of webcam textures
    private List<WebCamTexture> camList = new List<WebCamTexture>();

    // Gameobjects that need to be set in inspector menu
    public Dropdown skyBoxDropdown;
    public Material skyBoxMaterial;
    public GameObject display1;
    public GameObject display2;
    public GameObject display3;
    public GameObject display4;
    public GameObject display5;

    // List of display gameobjects and their components
    private GameObject[] displayArray = new GameObject[numDisplays]; 
    private RawImage[] backgroundArray = new RawImage[numDisplays]; 
    private AspectRatioFitter[] aspectRatioArray = new AspectRatioFitter[numDisplays];
    private Dropdown[] dropdownArray = new Dropdown[numDisplays];

    // List containing what camera index is displayed on each display
    // -1 if display is inactive
    private int[] displayCameraIdxs = new int[numDisplays];
    private int skyboxCameraIdx = -1; // Initialize skybox camera idx value

    // For dropdown menu
    List<string> cameraNameList = new List<string>();
    

    void Start()
    {
        // Save displays
        displayArray[0] = display1;
        displayArray[1] = display2;
        displayArray[2] = display3;
        displayArray[3] = display4;
        displayArray[4] = display5;

        for (int i = 0; i < numDisplays; i++)
        {
            backgroundArray[i] = displayArray[i].GetComponentInChildren(typeof(RawImage)) as RawImage;
            aspectRatioArray[i] = displayArray[i].GetComponentInChildren(typeof(AspectRatioFitter)) as AspectRatioFitter;
            dropdownArray[i] = displayArray[i].GetComponentInChildren(typeof(Dropdown)) as Dropdown;

            // Initialize all displays to inactive state 
            displayCameraIdxs[i] = -1;
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
        }

        // Update dropdown options
        skyBoxDropdown.ClearOptions();
        skyBoxDropdown.AddOptions(cameraNameList);
        for (int i = 0; i < numDisplays; i++)
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
            int prevCamIdx;

            if (displayIdx == -1) // Skybox camera feed
            {
                skyBoxMaterial.mainTexture = null;
                prevCamIdx = skyboxCameraIdx;
                skyboxCameraIdx = -1;
            }
            else // Non-skybox camera feed
            {
                backgroundArray[displayIdx].texture = null;
                prevCamIdx = displayCameraIdxs[displayIdx];
                displayCameraIdxs[displayIdx] = -1;
            }
            // Turn off camera if no other displays are using it
            if (prevCamIdx != -1) // Only run the check for valid cameras
            {
                // Check if the camera index is not being used by either the skybox or the other displays
                if (skyboxCameraIdx != prevCamIdx && !displayCameraIdxs.Contains(prevCamIdx))
                {
                    if (camList[prevCamIdx].isPlaying)
                    {
                        camList[prevCamIdx].Stop();
                    }
                        
                }
            }
        }

        // Update camera display if a camera is chosen
        else
        {
            int cameraIdx = dropdown.value - 1; // Skip [Select Camera] option

            if (displayIdx == -1) // Skybox camera feed
            {
                skyBoxMaterial.mainTexture = camList[cameraIdx];
                skyboxCameraIdx = cameraIdx;
            }
            else // Non-skybox camera feed
            {
                backgroundArray[displayIdx].texture = camList[cameraIdx];
                displayCameraIdxs[displayIdx] = cameraIdx;
            }

            // Play camera if not already playing
            if (!camList[cameraIdx].isPlaying)
            {
                camList[cameraIdx].Play();
            }
        }
    }

    void UpdateCameraTexture(int cameraIdx)
    {
        // Make sure camera is initialized
        if (camList[cameraIdx] != null)
        {
            float ratio = (float)camList[cameraIdx].width / (float)camList[cameraIdx].height;
            aspectRatioArray[cameraIdx].aspectRatio = ratio;
            float scaleY = camList[cameraIdx].videoVerticallyMirrored ? -1f : 1f;
            backgroundArray[cameraIdx].rectTransform.localScale = new Vector3(1f, scaleY, 1f);
            int orient = -camList[cameraIdx].videoRotationAngle;
            backgroundArray[cameraIdx].rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }

    void Update()
    {
        if (!camAvailable)
            return;

        // Only update textures currently in use
        foreach (int camIdx in displayCameraIdxs.Distinct())
        {
            // Ignore -1 indexed camera (placeholder for unused displays)
            if (camIdx != -1)
                UpdateCameraTexture(camIdx);
        }

        // Update texture for skybox if needed
        if (skyboxCameraIdx != -1 && !displayCameraIdxs.Contains(skyboxCameraIdx))
            UpdateCameraTexture(skyboxCameraIdx);
    }
}