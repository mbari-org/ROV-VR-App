using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DepthOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject depthTextGameObject;
    public GameObject pressureTextGameObject;
    public GameObject altitudeTextGameObject;

    private TextMeshProUGUI depth;
    private TextMeshProUGUI pressure;
    private TextMeshProUGUI altitude;

    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        depth = depthTextGameObject.GetComponent<TextMeshProUGUI>();
        pressure = pressureTextGameObject.GetComponent<TextMeshProUGUI>();
        altitude = altitudeTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        depth.SetText("Depth: " + listener.Depth.ToString());
        pressure.SetText("Pressure: " + listener.Pressure.ToString());
    }
}
