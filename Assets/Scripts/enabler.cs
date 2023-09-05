using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enabler : MonoBehaviour
{
    // Start is called before the first frame update
    private bool check = false;
    public GameObject DepthCanvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (DepthCanvas.activeSelf) DepthCanvas.SetActive(false);
            else DepthCanvas.SetActive(true);
        }
    }
}
