using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.XR.OpenVR.SimpleJSON;

public class PopupSettings : MonoBehaviour
{
    [Header("UI")]
    public Canvas SettingsCanvas;
    public Button CloseButton;
    public Button ConfirmGlobalSettingsButton;

    [Header("LCM")]
    public InputField LCMInputField;
    public LCMListener LCMListenerObject;

    [Header("PTGUI")]
    public Button ChooseFileButton;
    public Text PTGUIFilenameText;
    public Material skyboxMaterial;

    bool prevEscBool = false;
    bool currEscBool = false;

    // Settings 
    // TODO: Save these in a JSON
    string PTGUIFilename;
    string LCMURL;

    void Start()
    {
        ChooseFileButton.onClick.AddListener(ChooseFileCallback);
        CloseButton.onClick.AddListener(CloseCallback);
        ConfirmGlobalSettingsButton.onClick.AddListener(ConfirmGlobalSettingsCallback);


    }



    void Update()
    {
        currEscBool = Input.GetKey(KeyCode.Escape);

        // Press Escape to open/close window
        if (currEscBool == true && prevEscBool == false)
        {
            SettingsCanvas.enabled = !SettingsCanvas.enabled;
        }

        prevEscBool = currEscBool;
        
        
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ////Debug.Log(Input.mousePosition);

        //if (Input.GetMouseButtonDown(0))
        //    Debug.Log("Pressed primary button.");
    }

   
    void ChooseFileCallback()
    {
        PTGUIFilename = EditorUtility.OpenFilePanel("Select Calibration File", "", "pts");
        PTGUIFilenameText.text = PTGUIFilename;

        var fileContent = File.ReadAllBytes(PTGUIFilename);
        string jsonString = System.Text.Encoding.UTF8.GetString(fileContent);
        JSONNode PTGUISettings = JSON.Parse(jsonString);
        string test_a = PTGUISettings["contenttype"].Value;

        Debug.Log(test_a);

        //if (path.Length != 0)
        //{
        //    var fileContent = File.ReadAllBytes(path);
        //    texture.LoadImage(fileContent);
        //}
        //JsonTextReader reader = new JsonTextReader(new StringReader(json));
    }

    void CloseCallback()
    {
        SettingsCanvas.enabled = false;
    }

    void ConfirmGlobalSettingsCallback()
    {
        LCMURL = LCMInputField.text;
        Debug.Log(LCMURL);
        // TODO: Update LCM here
        // Create this function: LCMListenerObject.changeURL(LCMInputField.text)
    }
}
