using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class CameraSettingsManager : MonoBehaviour
{
    // Joystick bindings
    public SteamVR_Action_Vector2 ThumbstickAction;
    public SteamVR_Input_Sources RHand;

    // Skybox we'll be adjusting
    public Material skyboxMaterial;

    // Used for color palettes
    public Button InactiveButtonTemplate;
    public Button ActiveButtonTemplate;

    // GUI objects
    public Button LCXButton;
    public Text LCXButtonText;
    public Text LCXValueText;

    public Button LCYButton;
    public Text LCYButtonText;
    public Text LCYValueText;

    public Button RCXButton;
    public Text RCXButtonText;
    public Text RCXValueText;

    public Button RCYButton;
    public Text RCYButtonText;
    public Text RCYValueText;

    public Button LRXButton;
    public Text LRXButtonText;
    public Text LRXValueText;

    public Button LRYButton;
    public Text LRYButtonText;
    public Text LRYValueText;

    public Button RRXButton;
    public Text RRXButtonText;
    public Text RRXValueText;

    public Button RRYButton;
    public Text RRYButtonText;
    public Text RRYValueText;

    public Button FisheyeAButton;
    public Text FisheyeAButtonText;
    public Text FisheyeAValueText;

    public Button FisheyeBButton;
    public Text FisheyeBButtonText;
    public Text FisheyeBValueText;

    public Button FisheyeCButton;
    public Text FisheyeCButtonText;
    public Text FisheyeCValueText;

    // Keep items in a list to make it easier to use
    private List<Button> ButtonList;
    private List<Text> ButtonTextList;
    private List<Text> ValueTextList;
    private List<string> CameraSettingsList;

    int currActive = -1;



    // Start is called before the first frame update
    void Start()
    {
        ButtonList = new List<Button> { LCXButton, LCYButton, RCXButton,
            RCYButton, LRXButton, LRYButton, RRXButton, RRYButton, 
            FisheyeAButton, FisheyeBButton, FisheyeCButton };

        ButtonTextList = new List<Text> { LCXButtonText, LCYButtonText, RCXButtonText, 
            RCYButtonText, LRXButtonText, LRYButtonText, RRXButtonText, RRYButtonText, 
            FisheyeAButtonText, FisheyeBButtonText, FisheyeCButtonText };

        ValueTextList = new List<Text> { LCXValueText, LCYValueText, RCXValueText, 
            RCYValueText, LRXValueText, LRYValueText, RRXValueText, RRYValueText, 
            FisheyeAValueText, FisheyeBValueText, FisheyeCValueText };

        CameraSettingsList = new List<string> { "_L_CX", "_L_CY", "_R_CX",
            "_R_CY", "_L_RX", "_L_RY", "_R_RX", "_R_RY", 
            "_a", "_b", "_c" };

        LCXButton.onClick.AddListener(delegate { SettingsButtonCallback(0); });
        LCYButton.onClick.AddListener(delegate { SettingsButtonCallback(1); });
        RCXButton.onClick.AddListener(delegate { SettingsButtonCallback(2); });
        RCYButton.onClick.AddListener(delegate { SettingsButtonCallback(3); });
        LRXButton.onClick.AddListener(delegate { SettingsButtonCallback(4); });
        LRYButton.onClick.AddListener(delegate { SettingsButtonCallback(5); });
        RRXButton.onClick.AddListener(delegate { SettingsButtonCallback(6); });
        RRYButton.onClick.AddListener(delegate { SettingsButtonCallback(7); });
        FisheyeAButton.onClick.AddListener(delegate { SettingsButtonCallback(8); });
        FisheyeBButton.onClick.AddListener(delegate { SettingsButtonCallback(9); });
        FisheyeCButton.onClick.AddListener(delegate { SettingsButtonCallback(10); });

        // Populate GUI with initial values
        // TODO: Read these from a save file
        int i = 0;
        foreach (string settingName in CameraSettingsList)
        {
            float value = skyboxMaterial.GetFloat(settingName);
            skyboxMaterial.SetFloat(settingName, value);
            ValueTextList[i].text = settingName + ": " + value.ToString();
            i += 1;
        }
    }

    void Update()
    {
        if (currActive != -1)
        {
            // Use right joystick to update this value
            Vector2 joystickInput = ThumbstickAction.GetAxis(RHand);
            string settingName = CameraSettingsList[currActive];
            float newValue = skyboxMaterial.GetFloat(settingName) + joystickInput.y * 0.0003f; // 0.1 to adjust sensitivity
            skyboxMaterial.SetFloat(settingName, newValue);
            ValueTextList[currActive].text = settingName + ": " + newValue.ToString();
        }
    }

    void SettingsButtonCallback(int idx)
    {
        Button button = ButtonList[idx];
        Text buttonText = ButtonTextList[idx];
        Text valueText = ValueTextList[idx];


        if (currActive == idx)
        {
            // Button is currently active - we need to make it inactive
            ButtonList[currActive].colors = InactiveButtonTemplate.colors;
            ButtonTextList[currActive].text = "Click to adjust";
            currActive = -1; // Reset currActive value since nothing should be active
        }
        else if (currActive != -1)
        {
            // Some other button was just active - we need to make it inactive first
            ButtonList[currActive].colors = InactiveButtonTemplate.colors;
            ButtonTextList[currActive].text = "Click to adjust";

            button.colors = ActiveButtonTemplate.colors;
            buttonText.text = "Click to set";
            currActive = idx;
        } else
        {
            // All buttons were just inactive
            button.colors = ActiveButtonTemplate.colors;
            buttonText.text = "Click to set";
            currActive = idx;
        }
    }
}
