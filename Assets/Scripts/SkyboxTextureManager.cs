using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;

public class SkyboxTextureManager : MonoBehaviour
{
    public AVProLiveCamera leftAVProCamera;
    public AVProLiveCamera rightAVProCamera;
    public Material skyboxMaterial;

    void Update()
    {
        skyboxMaterial.SetTexture(Shader.PropertyToID("_LTex"), leftAVProCamera.OutputTexture);
        skyboxMaterial.SetTexture(Shader.PropertyToID("_RTex"), rightAVProCamera.OutputTexture);
    }
}
