using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSettingsManager : MonoBehaviour
{
    // Skybox we'll be adjusting
    public Material skyboxMaterial;

    // Used for color palettes
    public Button InactiveButtonTemplate;
    public Button ActiveButtonTemplate;

    public Button LCXButton;
    public Text LCXButtonText;
    public Text LCXValueText;

    // Keep items in a list to make it easier to use
    private List<Button> ButtonList;
    private List<Text> ButtonTextList;
    private List<Text> ValueTextList;
    private List<float> CameraSettingsList;

    int currActive = -1;



    // Start is called before the first frame update
    void Start()
    {
        ButtonList = new List<Button> { LCXButton };
        ButtonTextList = new List<Text> { LCXButtonText };
        ValueTextList = new List<Text> { LCXValueText };
        CameraSettingsList = new List<float> { 
            skyboxMaterial.GetFloat("_L_CX"),
            skyboxMaterial.GetFloat("_L_CY"),
            skyboxMaterial.GetFloat("_R_CX"),
            skyboxMaterial.GetFloat("_R_CY"),
            skyboxMaterial.GetFloat("_L_RX"),
            skyboxMaterial.GetFloat("_L_RY"),
            skyboxMaterial.GetFloat("_R_RX"),
            skyboxMaterial.GetFloat("_R_RY") };

        LCXButton.onClick.AddListener(delegate
            { SettingsButtonCallback(0); });


        //L_CX_slider.value = skyboxMaterial.GetFloat("_L_CX");
        //L_CY_slider.value = skyboxMaterial.GetFloat("_L_CY");
        //R_CX_slider.value = skyboxMaterial.GetFloat("_R_CX");
        //R_CY_slider.value = skyboxMaterial.GetFloat("_R_CY");
        //L_RX_slider.value = skyboxMaterial.GetFloat("_L_RX");
        //L_RY_slider.value = skyboxMaterial.GetFloat("_L_RY");
        //R_RX_slider.value = skyboxMaterial.GetFloat("_R_RX");
        //R_RY_slider.value = skyboxMaterial.GetFloat("_R_RY");

    }

    // Update is called once per frame
    void Update()
    {
        if (currActive != -1)
        {
            // Use right joystick to update this value
            //print(CameraSettingsList[currActive]);
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
