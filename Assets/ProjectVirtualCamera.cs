using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectVirtualCamera : MonoBehaviour
{
    public Material skyBoxMaterial; 
    // Start is called before the first frame update
    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        Renderer renderer = GetComponent<Renderer>();
        Debug.Log("Looking for camera named: OBS-Camera");
        webcamTexture.deviceName = "OBS-Camera";
        skyBoxMaterial.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
