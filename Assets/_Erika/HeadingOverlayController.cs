using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HeadingOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject barGameObject;
    public GameObject headingLabelGameObject;

    private TextMeshProUGUI headingText;
    private Transform barPosition;
    private float headingBarLength = 1620f; 
    private float offsetAdjustment = 0f; //to make arrow line up with values better
    private float curvedGUIAdjustment = 15f;
    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        headingText = headingLabelGameObject.GetComponent<TextMeshProUGUI>();
        barPosition = barGameObject.GetComponent<RectTransform>();
    }



    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = barPosition.position;

        targetPos.x = -1f * (float) (listener.Yaw/360f) * (headingBarLength + curvedGUIAdjustment) + offsetAdjustment;
        var diff = Mathf.Abs(barPosition.position.x - targetPos.x);
        var step = diff < 20 ? .1f : diff;
        barPosition.position =  Vector3.MoveTowards(barPosition.position, targetPos, step );
       
        headingText.SetText(Math.Round(listener.Yaw, 1).ToString());
    }
}
