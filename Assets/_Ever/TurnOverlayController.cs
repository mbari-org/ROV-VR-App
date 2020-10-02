using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Useful References for Ever:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html
// - https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html (for arrows)

public class TurnOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject compassGameObject;


    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Yaw: " + listener.Yaw.ToString());
    }
}
