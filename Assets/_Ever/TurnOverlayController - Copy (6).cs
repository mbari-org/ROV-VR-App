using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// Useful References for Ever:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html
// - https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html (for arrows)

public class TurnOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject compassGameObject;
    public GameObject turnLabelGameObject;

    private TextMeshProUGUI turnText;
    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        turnText = turnLabelGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Yaw: " + listener.Yaw.ToString());
        
        // // turnText.SetText("+" + Math.Round(1.2345678, 1).ToString());
    }
}
