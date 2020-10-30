using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class DeltaOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject deltaTextGameObject;

    private TextMeshProUGUI delta;

    private double deltaValue;


    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        delta = deltaTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaValue = Math.Round(listener.RopeLength - listener.Depth, 3);
        delta.SetText("Delta: " + deltaValue.ToString());
    }
}
