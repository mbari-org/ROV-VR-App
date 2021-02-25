using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSettings : MonoBehaviour
{
    public Canvas SettingsCanvas;


    bool prevEscBool = false;
    bool currEscBool = false;

    void Update()
    {
        currEscBool = Input.GetKey(KeyCode.Escape);

        // Press Escape to open/close window
        if (currEscBool == true && prevEscBool == false)
        {
            Debug.Log("here!");
            SettingsCanvas.enabled = !SettingsCanvas.enabled;
        }

        prevEscBool = currEscBool;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ////Debug.Log(Input.mousePosition);

        //if (Input.GetMouseButtonDown(0))
        //    Debug.Log("Pressed primary button.");
    }
}
