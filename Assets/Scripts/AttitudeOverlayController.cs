using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;//*


// Useful References for Cali:
// - https://docs.unity3d.com/ScriptReference/Transform.Rotate.html

public class AttitudeOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject cubeGameObject;

    public GameObject rollObject;
    public GameObject pitchObject;
    public GameObject yawObject;

    public Transform camTarget;

    private TextMesh roll, pitch, yaw;

    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();

        roll = rollObject.GetComponent<TextMesh>();
        pitch = pitchObject.GetComponent<TextMesh>();
        yaw = yawObject.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        //prints attitude data in unity console
/*        Debug.Log("Roll: " + listener.Roll.ToString());
        Debug.Log("Pitch: " + listener.Pitch.ToString());
        Debug.Log("Yaw: " + listener.Yaw.ToString());*/

        //Rotates cube to angles as described by LCM. changes doubles to floats because quaternion only accepts floats.
        cubeGameObject.transform.rotation = Quaternion.Euler((float)listener.Pitch, (float)listener.Yaw, (float)listener.Roll);

        roll.GetComponent<TextMesh>().text = "Roll: " + listener.Roll.ToString();
        pitch.GetComponent<TextMesh>().text = "Pitch: " + listener.Pitch.ToString();
        yaw.GetComponent<TextMesh>().text = "Yaw: " + listener.Yaw.ToString();

        //Rotates printed attitude text
        //yawObject.transform.rotation = Quaternion.Euler(-1 * (float)listener.Pitch, 180 -1 * (float)listener.Yaw, -1 * (float)listener.Roll);
        yawObject.transform.rotation = Quaternion.LookRotation(transform.position - camTarget.position);
        rollObject.transform.rotation = Quaternion.LookRotation(transform.position - camTarget.position);
        pitchObject.transform.rotation = Quaternion.LookRotation(transform.position - camTarget.position);
        //rollObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        //pitchObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        //yawObject.transform.LookAt(camTarget);
        //rollObject.transform.LookAt(camTarget);
        //pitchObject.transform.LookAt(camTarget);
    }
}
