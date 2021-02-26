using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.XR.OpenVR.SimpleJSON;

public class OverlaySettings : MonoBehaviour
{
    public float overlayName;
    public bool isVisible;
    public Transform tf;
}
public class User : MonoBehaviour
{

    public List<OverlaySettings> overlaySettings;
    public string userName;

    public User(string name)
    {
        userName = name;
    }

    void RecordOverlaySettings(List<GameObject> overlays)
    {
        /*
         * for overlay in overlays:
         *      OverlaySettings newOverlay;
         *          save name, enabled, and recttransform to object
         *      add object to overlaySettings
         */
    }

    void RestoreOverlaySettings(List<GameObject> overlays)
    {
        /*
         * for overlay in overlays:
         *      find matching name
         *      set enabled, and recttransform based on overlaySettings
         */
    }

}
public class PopupSettings : MonoBehaviour
{
    [Header("Settings UI")]
    public Canvas SettingsCanvas;
    public Button CloseButton;
    public Button ConfirmGlobalSettingsButton;

    [Header("User")]
    public Dropdown ChooseUserDropdown;
    public InputField NewUserInputField;
    public Button AddUserButton;

    [Header("LCM")]
    public InputField LCMInputField;
    public LCMListener LCMListenerObject;

    [Header("PTGUI")]
    public Button ChooseFileButton;
    public Text PTGUIFilenameText;
    public Material skyboxMaterial;

    [Header("Overlays")]
    // TODO: Include all the overlays here so we can save their xy coords

    bool prevEscBool = false;
    bool currEscBool = false;

    // Settings 
    // TODO: Save these in a JSON
    List<User> users = new List<User> { }; // TODO: finish implementing this class
    [HideInInspector]
    public string PTGUIFilename;
    [HideInInspector]
    public string LCMURL;

    void Start()
    {
        // TODO: When we implement load settings, remember to update LCM URL at start

        ChooseFileButton.onClick.AddListener(ChooseFileCallback);
        CloseButton.onClick.AddListener(CloseCallback);
        ConfirmGlobalSettingsButton.onClick.AddListener(ConfirmGlobalSettingsCallback);
        AddUserButton.onClick.AddListener(AddUserCallback);
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
        LCMListenerObject.changeURL(LCMURL);
    }

    void AddUserCallback()
    {
        // TODO: Prevent duplicate names, ensure name is not empty
        // Create new user
        User newUser = new User(NewUserInputField.text);
        users.Add(newUser);

        // Add new user name to drop down, select new user
        ChooseUserDropdown.AddOptions(new List<string> { newUser.userName });
        int userIdx = ChooseUserDropdown.options.FindIndex(ChooseUserDropdown => ChooseUserDropdown.text == newUser.userName);
        ChooseUserDropdown.value = userIdx;

        // Clear text when done
        NewUserInputField.text = "";

    }
}
