using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DepthOverlayController : MonoBehaviour
{
    public GameObject listenerGameObject;
    private LCMListener listener;

    public GameObject depthTextGameObject;
    public GameObject arrowGameObject;

    public Sprite dotSprite;

    private TextMeshProUGUI depth;


    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();
        depth = depthTextGameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        depth.SetText("Depth: " + listener.Depth.ToString());
    }
}
