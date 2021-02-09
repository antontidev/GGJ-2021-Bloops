using Cinemachine;
using UnityEngine;
using MyBox;

[CreateAssetMenu(menuName = "GameSettings/Settinngs")]
public class SharedPlatformSettings : GameSettings
{
    public bool joystickVisibility;

    public int cameraOrtographicSize;

    [ReadOnly]
    public int editorOrtographicSize = 7;

    public void ProvideJoystick(GameObject joystick) {
        joystick.SetActive(joystickVisibility);
    }

    public void ProvideCinemachineCamera(CinemachineVirtualCamera vCam) {
        vCam.m_Lens.OrthographicSize = cameraOrtographicSize;
    }
}
