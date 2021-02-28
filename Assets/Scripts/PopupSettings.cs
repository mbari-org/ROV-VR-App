using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Unity.XR.OpenVR.SimpleJSON;

[System.Serializable]
public class OverlayConfig
{
    public string overlayName;
    public bool isVisible;
    public float xPosition;
    public float yPosition;

    public void recordOverlayConfig(GameObject overlay)
    {
        overlayName = overlay.name;
        isVisible = overlay.activeInHierarchy;
        xPosition = overlay.transform.localPosition.x;
        yPosition = overlay.transform.localPosition.y;
    }
}

[System.Serializable]
public class User
{
    public List<OverlayConfig> overlaySettings = new List<OverlayConfig> { };
    public string userName;

    public User(string name)
    {
        userName = name;
    }

    public void RecordOverlaySettings(List<GameObject> overlays)
    {
        foreach(GameObject overlay in overlays)
        {
            OverlayConfig newOverlay = new OverlayConfig();
            newOverlay.recordOverlayConfig(overlay);
            overlaySettings.Add(newOverlay);
        }
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

[System.Serializable]
public class RovVrSettings
{
    // Settings 
    // TODO: Save these in a JSON
    public List<User> users = new List<User> { }; // TODO: finish implementing this class
    public string PTGUIFilename;
    public string LCMURL;
}

public class SaveData
{
    private RovVrSettings applicationSettings;
    string saveFilePath;

    public SaveData(RovVrSettings settings, string filepath)
    {
        applicationSettings = settings;
        saveFilePath = filepath;
        SaveIntoJson();
    }
    
    public void SaveIntoJson()
    {
        string serializedSettings = JsonUtility.ToJson(applicationSettings);
        System.IO.File.WriteAllText(saveFilePath, serializedSettings);
    }
}



public class PopupSettings : MonoBehaviour
{
    [Header("Settings UI")]
    public Canvas SettingsCanvas;
    public Button CloseButton;
    public Button SaveAllButton;

    [Header("User")]
    public Dropdown ChooseUserDropdown;
    public InputField NewUserInputField;
    public Button AddUserButton;
    public Button LoadUserSettingsButton;
    public Button WriteUserSettingsButton;

    [Header("LCM")]
    public InputField LCMInputField;
    public LCMListener LCMListenerObject;

    [Header("PTGUI")]
    public InputField PTGUIFilepathInputField;
    public Material skyboxMaterial;

    [Header("Overlays")]
    public GameObject mainOverlayCanvas;
    // TODO: Include all the overlays here so we can save their xy coords

    // TODO: load in individaul gui gameobjects
    // TODO: create a list of gui gameobjects
    // TODO: modify WriteUserSettingsCallback to take in this list 
    // TODO: WE LEFT OFF HERE

    RovVrSettings settings = new RovVrSettings();

    string saveFilePath;
    int currUserIdx = -1;
    bool prevEscBool = false;
    bool currEscBool = false;
    List<GameObject> overlayList;


    void Start()
    {
        // TODO: When we implement load settings, remember to update LCM URL at start

        CloseButton.onClick.AddListener(CloseCallback);
        AddUserButton.onClick.AddListener(AddUserCallback);
        SaveAllButton.onClick.AddListener(SaveAllCallback);
        WriteUserSettingsButton.onClick.AddListener(WriteUserSettingsCallback);
        LoadUserSettingsButton.onClick.AddListener(LoadUserSettingsCallback);

        ChooseUserDropdown.onValueChanged.AddListener(delegate 
            { ChooseUserCallback(ChooseUserDropdown); });

        // Get all overlay gameobjects from mainOverlayCanvas
        Transform[] allChildren = mainOverlayCanvas.GetComponentsInChildren<Transform>(includeInactive: true);
        overlayList = new List<GameObject> { };

        foreach (Transform child in allChildren)
        {
            if (child.parent == mainOverlayCanvas.transform)
            {
                GameObject overlay = child.gameObject;
                overlayList.Add(overlay);
            }
        }

        // Load saved settings
        saveFilePath = Application.persistentDataPath + "/ROV-VR_Application_Settings.json";
        LoadSavedSettings();
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

    void CloseCallback()
    {
        SettingsCanvas.enabled = false;
    }

    void AddUserCallback()
    {
        // TODO: Prevent duplicate names, ensure name is not empty
        // Create new user
        User newUser = new User(NewUserInputField.text);
        settings.users.Add(newUser);

        // Add new user name to drop down, select new user
        ChooseUserDropdown.AddOptions(new List<string> { newUser.userName });
        int userIdx = ChooseUserDropdown.options.FindIndex(ChooseUserDropdown => ChooseUserDropdown.text == newUser.userName);
        ChooseUserDropdown.value = userIdx;
        currUserIdx = userIdx;

        // Clear text when done
        NewUserInputField.text = "";
    }

    void SaveAllCallback()
    {
        // Save LCM URL
        settings.LCMURL = LCMInputField.text;
        LCMListenerObject.changeURL(settings.LCMURL);

        // Save PTGUI Filepath
        settings.PTGUIFilename = PTGUIFilepathInputField.text;

        // TODO: Implement file reading and extract ptgui settings
        //var fileContent = File.ReadAllBytes(settings.PTGUIFilename);
        //string jsonString = System.Text.Encoding.UTF8.GetString(fileContent);
        //JSONNode PTGUISettings = JSON.Parse(jsonString);
        //string test_a = PTGUISettings["contenttype"].Value;

        print("Attempting to save data");
        SaveData dataSaver = new SaveData(settings, saveFilePath);
    }

    void WriteUserSettingsCallback()
    {
        if (currUserIdx == -1)
        {
            Debug.LogWarning("No user selected. Overlay settings will not be saved");
            return;
        }

        settings.users[currUserIdx].RecordOverlaySettings(overlayList);
    }

    void ChooseUserCallback(Dropdown ChooseUserDropdown)
    {
        int i = ChooseUserDropdown.value;
        currUserIdx = i;
    }

    void LoadSavedSettings()
    {
        if (File.Exists(saveFilePath))
        {
            // Read the entire file and save its contents.
            string fileContents = File.ReadAllText(saveFilePath);

            // Deserialize the JSON data
            settings = JsonUtility.FromJson<RovVrSettings>(fileContents);

            // Update the UI
            // Create a list containing all user names
            List<string> userNameList = new List<string> { };
            foreach (User user in settings.users)
                userNameList.Add(user.userName);

            currUserIdx = 0; // TODO: add checking to make sure there are actually users

            // Update Dropdown Options
            ChooseUserDropdown.AddOptions(userNameList);
            ChooseUserDropdown.value = currUserIdx; 

            // Update LCM InputField
            LCMInputField.text = settings.LCMURL;
            PTGUIFilepathInputField.text = settings.PTGUIFilename;

            // Load default player settings
            LoadUserSettingsCallback();
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }

    void LoadUserSettingsCallback()
    {
        foreach (GameObject overlay in overlayList)
        {
            foreach (OverlayConfig overlayConfig in settings.users[currUserIdx].overlaySettings)
            {
                if (overlay.name == overlayConfig.overlayName)
                {
                    Vector3 newPos = new Vector3(overlayConfig.xPosition, overlayConfig.yPosition, 0);
                    overlay.transform.localPosition = newPos;
                    overlay.SetActive(overlayConfig.isVisible);
                }
            }
        }
    }
}
