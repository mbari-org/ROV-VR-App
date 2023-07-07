using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;

public class SkyboxTextureManager : MonoBehaviour
{
    public AVProLiveCamera leftAVProCamera;
    public AVProLiveCamera rightAVProCamera;
    public Material skyboxMaterial;
  //  public bool check=false;

    void Update()
    {
        
        if (Input.GetKeyDown("space"))
        {
            // check = true;
            UnityEngine.Debug.Log("Pressed");
            CreateImageFiles(leftAVProCamera.OutputTexture, 1);
            CreateImageFiles(rightAVProCamera.OutputTexture, 2);
            run_cmd();
            AssetDatabase.Refresh();
        }

        
        //Debug.Log("SkyBoxTextureManager");
        skyboxMaterial.SetTexture(Shader.PropertyToID("_LTex"), leftAVProCamera.OutputTexture);
        skyboxMaterial.SetTexture(Shader.PropertyToID("_RTex"), rightAVProCamera.OutputTexture);
        //CreateImageFiles(leftAVProCamera.OutputTexture, 1);
        //CreateImageFiles(rightAVProCamera.OutputTexture, 2);




    }



    private static void CreateImageFiles(Texture tex, int index)
    {
        //Shoutout to https://discussions.unity.com/t/convert-texture-to-texture2d/203896
        Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 32);
        Graphics.Blit(tex, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        Color[] pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;


       // Texture2D twoD = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
       // Texture2D twoD = (Texture2D)tex;
        var pngData = texture2D.EncodeToJPG();
        if (pngData.Length < 1)
        {
            return;
        }
        var path = @"C:\Users\Benjamin\Documents\ROV-VR-App\Assets\Depth\img" + index + ".jpg";
        File.WriteAllBytes(path, pngData);
        AssetDatabase.Refresh();
    }

    private void run_cmd()
    {
        string fileName = @"C:\Users\Benjamin\Documents\ROV-VR-App\Assets\Depth\halo.py";

        Process p = new Process();
        p.StartInfo = new ProcessStartInfo(@"C:\Program Files\Python310\python.exe", fileName)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        p.Start();
        UnityEngine.Debug.Log("start");
        string output = p.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(output);
        p.WaitForExit();
        UnityEngine.Debug.Log("exit");
    }

}
