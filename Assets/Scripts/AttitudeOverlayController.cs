using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttitudeOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject rollTextGameObject;
    public GameObject pitchTextGameObject;
    public GameObject yawTextGameObject;

    private TextMeshProUGUI roll;
    private TextMeshProUGUI pitch;
    private TextMeshProUGUI yaw;



    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        roll = rollTextGameObject.GetComponent<TextMeshProUGUI>();
        pitch = pitchTextGameObject.GetComponent<TextMeshProUGUI>();
        yaw = yawTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        roll.SetText("Roll: " + listener.Roll.ToString());
        pitch.SetText("Pitch: " + listener.Pitch.ToString());
        yaw.SetText("Yaw: " + listener.Yaw.ToString());
    }
}
