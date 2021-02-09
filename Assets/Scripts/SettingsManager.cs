using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsManager {
    void ApplyAllSettings();
}

public class SettingsManager : MonoBehaviour, ISettingsManager
{
    [Header("Settings files")]
    [SerializeField]
    private SharedPlatformSettings mobileSettings;

    [SerializeField]
    private SharedPlatformSettings standaloneSettings;

    [Header("Dependent objects")]
    [SerializeField]
    private GameObject joystickCanvas;

    public CinemachineVirtualCamera vCam;

    private SharedPlatformSettings currentSettings;

    public void ApplyAllSettings() {
        currentSettings.ProvideCinemachineCamera(vCam);
        currentSettings.ProvideJoystick(joystickCanvas);
    }

    public void ApplyJoystickSettings() {
        currentSettings.ProvideJoystick(joystickCanvas);
    }

    public void ApplyCinemachineSettings() {
        currentSettings.ProvideCinemachineCamera(vCam);
    }

    private void Awake() {
#if UNITY_EDITOR 
        currentSettings = mobileSettings;
#elif UNITY_STANDALONE || UNITY_WEBGL
        currentSettings = standaloneSettings;
#elif UNITY_ANDROID || UNITY_IPHONE
        currentSettings = mobileSettings;
#endif
    }
}
