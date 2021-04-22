using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;

public class TextureCopier : MonoBehaviour
{
    public AVProLiveCamera AVProCamera;
    public Material skyboxMaterial;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        skyboxMaterial.SetTexture(Shader.PropertyToID("_LTex"), AVProCamera.OutputTexture);
    }
}
