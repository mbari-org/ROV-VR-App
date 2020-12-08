using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WebCam : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture cam1;
    private Texture defaultBackground1;
    public RawImage background1;
    public AspectRatioFitter fit1;
    private int camIndex = 0;
    private int magicCam = 0;//-1
    //cameras 2-5
    private WebCamTexture cam2;
    private WebCamTexture cam3;
    private WebCamTexture cam4;
    private WebCamTexture cam5;
    private Texture defaultBackground2;
    private Texture defaultBackground3;
    private Texture defaultBackground4;
    private Texture defaultBackground5;
    public RawImage background2;
    public RawImage background3;
    public RawImage background4;
    public RawImage background5;
    public AspectRatioFitter fit2;
    public AspectRatioFitter fit3;
    public AspectRatioFitter fit4;
    public AspectRatioFitter fit5;

    void Start()
    {
        defaultBackground1 = background1.texture;
        //cameras 2-5
        defaultBackground2 = background2.texture;
        defaultBackground3 = background3.texture;
        defaultBackground4 = background4.texture;
        defaultBackground5 = background5.texture;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("no camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            /*if (devices[i].name != "Leap Dev Kit")
            {
                camIndex = i;
                if (magicCam == -1)
                {
                    magicCam = 0;
                }
            }*/
            /*if (devices[i].name.Contains("Black"))
            {
                camIndex = i;
            if (magicCam == -1)
                {
                    magicCam = 0;
                }
            }*/
            if (devices[i].name.Contains("Epoc")) //Caps don't matter
            {
                camIndex = i;
                if (magicCam == -1)
                {
                    magicCam = 0;
                }
            }

            Debug.Log(devices[i].name);
            Debug.Log(i);

            if (magicCam == 0)
            {
                if (cam1 == null)
                {
                    cam1 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
                    magicCam = 1;
                }
                if (!cam1.isPlaying)
                {
                    cam1.Play();
                    Debug.Log(cam1.isPlaying);
                }
                background1.texture = cam1;
            }
            if (magicCam == 1)
            {
                if (cam2 == null)
                {
                    cam2 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
                    magicCam = 2;
                }
                if (!cam2.isPlaying)
                {
                    cam2.Play();
                    Debug.Log(cam1.isPlaying);
                }
                background2.texture = cam2;
            }
            if (magicCam == 2)
            {
                if (cam3 == null)
                {
                    cam3 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
                    magicCam = 3;
                }
                if (!cam3.isPlaying)
                {
                    cam3.Play();
                    Debug.Log(cam1.isPlaying);
                }
                background3.texture = cam3;
            }
            if (magicCam == 3)
            {
                if (cam4 == null)
                {
                    cam4 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
                    magicCam = 4;
                }
                if (!cam4.isPlaying)
                {
                    cam4.Play();
                    Debug.Log(cam1.isPlaying);
                }
                background4.texture = cam4;
            }
            if (magicCam == 4)
            {
                if (cam5 == null)
                {
                    cam5 = new WebCamTexture(devices[camIndex].name, Screen.width, Screen.height);
                    magicCam = 5;
                }
                if (!cam5.isPlaying)
                {
                    cam5.Play();
                    Debug.Log(cam1.isPlaying);
                }
                background5.texture = cam5;
            }
        }

       /* if (!cam1.isPlaying)
        {
            cam1.Play();
            Debug.Log(cam1.isPlaying);
        }

        background1.texture = cam1;*/

        camAvailable = true;

    }
    void Update()
    {
        if (!camAvailable)
            return;

        //All cameras same size?
        if (magicCam >= 0)
        {
            float ratio1 = (float)cam1.width / (float)cam1.height;
            fit1.aspectRatio = ratio1;
            float scaleY1 = cam1.videoVerticallyMirrored ? -1f : 1f;
            background1.rectTransform.localScale = new Vector3(1f, scaleY1, 1f);
            int orient1 = -cam1.videoRotationAngle;
            background1.rectTransform.localEulerAngles = new Vector3(0, 0, orient1);
        }

        //cameras 2-5
        if (magicCam >= 1)
        {
            float ratio2 = (float)cam2.width / (float)cam2.height;
            fit2.aspectRatio = ratio2;
            float scaleY2 = cam2.videoVerticallyMirrored ? -1f : 1f;
            background2.rectTransform.localScale = new Vector3(1f, scaleY2, 1f);
            int orient2 = -cam2.videoRotationAngle;
            background2.rectTransform.localEulerAngles = new Vector3(0, 0, orient2);
        }
        if (magicCam >= 2)
        {
            float ratio3 = (float)cam3.width / (float)cam3.height;
            fit3.aspectRatio = ratio3;
            float scaleY3 = cam3.videoVerticallyMirrored ? -1f : 1f;
            background3.rectTransform.localScale = new Vector3(1f, scaleY3, 1f);
            int orient3 = -cam3.videoRotationAngle;
            background3.rectTransform.localEulerAngles = new Vector3(0, 0, orient3);
        }
        if (magicCam >= 3)
        {
            float ratio4 = (float)cam4.width / (float)cam4.height;
            fit4.aspectRatio = ratio4;
            float scaleY4 = cam4.videoVerticallyMirrored ? -1f : 1f;
            background4.rectTransform.localScale = new Vector3(1f, scaleY4, 1f);
            int orient4 = -cam4.videoRotationAngle;
            background4.rectTransform.localEulerAngles = new Vector3(0, 0, orient4);
        }
        if (magicCam >= 4)
        {
            float ratio5 = (float)cam5.width / (float)cam5.height;
            fit5.aspectRatio = ratio5;
            float scaleY5 = cam5.videoVerticallyMirrored ? -1f : 1f;
            background5.rectTransform.localScale = new Vector3(1f, scaleY5, 1f);
            int orient5 = -cam5.videoRotationAngle;
            background5.rectTransform.localEulerAngles = new Vector3(0, 0, orient5);
        }
    }
}