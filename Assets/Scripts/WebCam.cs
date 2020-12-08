using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WebCam : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture cam1;
    private Texture defaultBackground;
    private int camIndex = 0;
    public RawImage background;
    public AspectRatioFitter fit;

    void Start()
    {
        defaultBackground = background.texture;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("no camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name != "Leap Dev Kit")
            {
                camIndex = i;
            }
    
            Debug.Log(devices[i].name);
            Debug.Log(i);

        }

        if (cam1 == null)
        {
            cam1 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
        }

        if (!cam1.isPlaying)
        {
            cam1.Play();
            Debug.Log(cam1.isPlaying);
        }

        background.texture = cam1;

        camAvailable = true;

    }
    void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)cam1.width / (float)cam1.height;
        fit.aspectRatio = ratio;
        float scaleY = cam1.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -cam1.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
    }
}