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
        Debug.Log("Looking for camera named: Blackmagic Web Presenter");
        webcamTexture.deviceName = "Blackmagic Web Presenter";
        skyBoxMaterial.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
