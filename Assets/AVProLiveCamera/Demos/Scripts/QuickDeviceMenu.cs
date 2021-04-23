using UnityEngine;
using System.Collections.Generic;
using RenderHeads.Media.AVProLiveCamera;


public class QuickDeviceMenu : MonoBehaviour
{
	public AVProLiveCamera _leftLiveCamera;
	public AVProLiveCamera _rightLiveCamera;
	public AVProLiveCameraManager _liveCameraManager;
	public GUISkin _guiSkin;
	private Vector2 _scrollResolutions = Vector2.zero;
	private bool _isHidden = false;


    private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			ToggleVisible();
		}
	}

	private void ToggleVisible()
	{
		_isHidden = !_isHidden;
		this.useGUILayout = !_isHidden;		// NOTE: this reduces garbage generation to zero
	}

	void OnGUI()
	{
		if (_isHidden)
		{
			return;
		}

		GUI.skin = _guiSkin;

		if (_liveCameraManager.NumDevices > 0)
		{
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
            if (GUILayout.Button("Press TAB to hide/show QuickDeviceMenu (improves performance)", GUILayout.ExpandWidth(false)))
			{
				ToggleVisible();
			}
            GUILayout.Button("SHIFT-click to make right camera selection", GUILayout.ExpandWidth(false));

            GUILayout.EndArea();

            // NOTE: This is just a spacing element to leave space for the above message
            GUILayout.Label(" ");
            GUILayout.Label(" ");

            GUILayout.BeginHorizontal();

            // Select device
            GUILayout.BeginVertical();
            GUILayout.Button("SELECT DEVICE");
            for (int i = 0; i < _liveCameraManager.NumDevices; i++)
            {
                string name = _liveCameraManager.GetDevice(i).Name;

                GUI.color = Color.white;
                if (_leftLiveCamera.Device != null && _leftLiveCamera.Device.IsRunning)
                {
                    if (_leftLiveCamera.Device.DeviceIndex == i)
                    {
                        GUI.color = Color.cyan;
                    } 
                }
                if (_rightLiveCamera.Device != null && _rightLiveCamera.Device.IsRunning)
                {
                    if (_rightLiveCamera.Device.DeviceIndex == i)
                    {
                        GUI.color = Color.green;
                    }
                }

                if (GUILayout.Button(i.ToString() + ": " + name))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        _rightLiveCamera._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
                        _rightLiveCamera._desiredDeviceIndex = i;
                        _rightLiveCamera.Begin();
                    } 
                    else
                    {
                        _leftLiveCamera._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
                        _leftLiveCamera._desiredDeviceIndex = i;
                        _leftLiveCamera.Begin();
                    }
                }
            }
            GUI.color = Color.white;
            GUILayout.EndVertical();
            
            DisplaySettings(_rightLiveCamera, "RIGHT");
            DisplaySettings(_leftLiveCamera, "LEFT");
            
            GUILayout.EndHorizontal();
        }
		else
		{
			GUILayout.Label("No webcam / capture devices found");
		}
	}

    private void DisplaySettings(AVProLiveCamera liveCamera, string side)
    {
        Color activeButtonColor;
        if (side == "RIGHT")
        {
            activeButtonColor = Color.green;
        }
        else // side == "LEFT"
        {
            activeButtonColor = Color.cyan;
        }

        if (liveCamera.Device != null && liveCamera.Device.IsRunning)
        {
            GUILayout.BeginVertical();
            GUILayout.Button(side + " RESOLUTION");
            _scrollResolutions = GUILayout.BeginScrollView(_scrollResolutions, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            List<string> usedNames = new List<string>(32);
            for (int i = 0; i < liveCamera.Device.NumModes; i++)
            {
                AVProLiveCameraDeviceMode mode = liveCamera.Device.GetMode(i);
                string name = string.Format("{0}x{1}", mode.Width, mode.Height);
                if (!usedNames.Contains(name))
                {
                    GUI.color = Color.white;
                    if (liveCamera.Device.CurrentWidth == mode.Width && liveCamera.Device.CurrentHeight == mode.Height)
                    {
                        GUI.color = activeButtonColor;
                    }

                    usedNames.Add(name);
                    if (GUILayout.Button(name))
                    {
                        liveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
                        liveCamera._desiredModeIndex = i;
                        liveCamera.Begin();
                    }
                }
            }
            GUI.color = Color.white;
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            // Select frame rate
            usedNames.Clear();
            GUILayout.BeginVertical();
            GUILayout.Button(side + " FPS");
            for (int i = 0; i < liveCamera.Device.NumModes; i++)
            {
                string matchName = string.Format("{0}x{1}", liveCamera.Device.CurrentWidth, liveCamera.Device.CurrentHeight);

                AVProLiveCameraDeviceMode mode = liveCamera.Device.GetMode(i);

                string resName = string.Format("{0}x{1}", mode.Width, mode.Height);
                if (resName == matchName)
                {
                    foreach (float frameRate in mode.FrameRates)
                    {
                        string name = string.Format("{0}", frameRate.ToString("F2"));
                        if (!usedNames.Contains(name))
                        {
                            GUI.color = Color.white;
                            if (liveCamera.Device.CurrentFrameRate.ToString("F2") == frameRate.ToString("F2"))
                            {
                                GUI.color = activeButtonColor;
                            }

                            usedNames.Add(name);
                            if (GUILayout.Button(name))
                            {
                                liveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
                                liveCamera._desiredModeIndex = i;
                                liveCamera._desiredFrameRate = frameRate;
                                liveCamera.Begin();
                            }
                        }
                    }
                }
            }
            GUI.color = Color.white;
            GUILayout.EndVertical();

            // Select format
            usedNames.Clear();
            GUILayout.BeginVertical();
            GUILayout.Button(side + " FORMAT");
            for (int i = 0; i < liveCamera.Device.NumModes; i++)
            {
                string matchName = string.Format("{0}x{1}", liveCamera.Device.CurrentWidth, liveCamera.Device.CurrentHeight);//, liveCamera.Device.CurrentFrameRate.ToString("F2"));

                AVProLiveCameraDeviceMode mode = liveCamera.Device.GetMode(i);

                string resName = string.Format("{0}x{1}", mode.Width, mode.Height);//, mode.FPS.ToString("F2"));
                if (resName == matchName)
                {
                    string name = string.Format("{0}", mode.Format);
                    if (!usedNames.Contains(name))
                    {
                        GUI.color = Color.white;
                        if (liveCamera.Device.CurrentDeviceFormat == mode.Format)
                        {
                            GUI.color = activeButtonColor;
                        }

                        usedNames.Add(name);
                        if (GUILayout.Button(name))
                        {
                            liveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
                            liveCamera._desiredModeIndex = i;
                            liveCamera.Begin();
                        }
                    }
                }
            }
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }
    }
}