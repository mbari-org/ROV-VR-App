using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Useful References for Cali:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html

public class AttitudeOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject cubeGameObject;



    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Roll: " + listener.Roll.ToString());
        // Debug.Log("Pitch: " + listener.Pitch.ToString());
        // Debug.Log("Yaw: " + listener.Yaw.ToString());
    }
}
