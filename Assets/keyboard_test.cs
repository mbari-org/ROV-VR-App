using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboard_test : MonoBehaviour
{
    public bool check;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            check = true;
            Debug.Log("Pressed");
        //    CreateImageFiles(leftAVProCamera.OutputTexture, 1);
          //  CreateImageFiles(rightAVProCamera.OutputTexture, 2);
        }
    }
}
