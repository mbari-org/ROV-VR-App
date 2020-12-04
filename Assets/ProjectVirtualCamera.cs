using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectVirtualCamera : MonoBehaviour
{
    public Material skyBoxMaterial; 
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name + i);
        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        Debug.Log("Playing Camera: "+ devices[1].name);
        webcamTexture.deviceName = devices[1].name;
        skyBoxMaterial.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
