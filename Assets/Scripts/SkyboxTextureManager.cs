using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class SkyboxTextureManager : MonoBehaviour
{
    public AVProLiveCamera leftAVProCamera;
    public AVProLiveCamera rightAVProCamera;
    public Material skyboxMaterial;
    //  public bool check=false;
    private int i = 0;
    byte[] dataL = null;
    byte[] dataR = null;
  //  string assetsPath = Application.dataPath + "/Depth/disparity.jpg";

    void Update()
    {
//AssetDatabase.ImportAsset(assetsPath);

        if (Input.GetKeyDown("space"))
        {

            dataL=CreateImageFiles(leftAVProCamera.OutputTexture);
            dataR=CreateImageFiles(rightAVProCamera.OutputTexture);
            // check = true;
            UnityEngine.Debug.Log("Pressed");
            

            var t = Task.Run(() => writePNG(dataL,1,false));
            var r = Task.Run(() => writePNG(dataR,2, true));
            //AssetDatabase.ImportAsset(assetsPath);
            //t.Wait();
            //.Wait();

            // check = true;);
            //   CreateImageFiles(leftAVProCamera.OutputTexture, 1.ToString());
            //   CreateImageFiles(rightAVProCamera.OutputTexture, 2.ToString());
            //    run_cmd();
                AssetDatabase.Refresh();
        }


        //Debug.Log("SkyBoxTextureManager");
        // CreateImageFiles(leftAVProCamera.OutputTexture, "L"+i.ToString());
        // CreateImageFiles(rightAVProCamera.OutputTexture, "R"+i.ToString());
        skyboxMaterial.SetTexture(Shader.PropertyToID("_LTex"), leftAVProCamera.OutputTexture);
        skyboxMaterial.SetTexture(Shader.PropertyToID("_RTex"), rightAVProCamera.OutputTexture);
        //CreateImageFiles(leftAVProCamera.OutputTexture, 1);
        //CreateImageFiles(rightAVProCamera.OutputTexture, 2);




    }

    private static void writePNG(byte[] pngData, int index, bool T)
    {
        var path = @"C:\Users\Benjamin\Documents\ROV-VR-App\Assets\Depth\img" + index + ".jpg";
        var assetsPath = Application.dataPath + "/Depth/disparity.jpg";
        File.WriteAllBytes(path, pngData);

         void run_cmd()
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
            AssetDatabase.ImportAsset(assetsPath);
        }

        if (T) run_cmd();
     //   AssetDatabase.Refresh();
    }

    private static byte[] CreateImageFiles(Texture tex)
    {
        //Shoutout to https://discussions.unity.com/t/convert-texture-to-texture2d/203896
        Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 32);
        Graphics.Blit(tex, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

       // Color[] pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;


       // Texture2D twoD = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
       // Texture2D twoD = (Texture2D)tex;
        var pngData = texture2D.EncodeToJPG();
        if (pngData.Length < 1)
        {

           // return;
        }
        return pngData;

    }



}
