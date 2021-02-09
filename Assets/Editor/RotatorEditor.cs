using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraRotator))]
public class RotatorEditor : Editor
{
    private CameraRotator serializedProperty;
    private bool editMode = false;

    private Quaternion oldRotation;
    private Transform oldLookAt;
    private Transform oldFollow;

    private CinemachineBrain cinemachineBrain;

    private void OnEnable() {
        serializedProperty = target as CameraRotator;
    }

    private void OnDisable() {
        if (oldFollow != null && oldLookAt != null) {
            var activeVirtualCamera = GetActiveVirtualCamera();

            RestoreCamera(activeVirtualCamera);
        }
    }

    private CinemachineVirtualCamera GetActiveVirtualCamera() {
        if (cinemachineBrain == null) {
            cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        return cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUIStyle buttonStyleBold;
        buttonStyleBold = new GUIStyle(GUI.skin.button);
        buttonStyleBold.fixedHeight = 22;
        buttonStyleBold.fontStyle = FontStyle.Bold;

        GUIStyle saveButtonStyleBold;
        saveButtonStyleBold = new GUIStyle(GUI.skin.button);
        saveButtonStyleBold.fixedHeight = 22;
        saveButtonStyleBold.fontStyle = FontStyle.Bold;
        saveButtonStyleBold.normal.textColor = Color.red;

        var needEnterSpecific = serializedProperty.IsEnter && serializedProperty.enterState == CameraRotator.State.Specific;
        var needExitSpecific = serializedProperty.IsExit && serializedProperty.exitState == CameraRotator.State.Specific;

        if (needEnterSpecific || needExitSpecific) {
            editMode = GUILayout.Toggle(editMode, "Edit Camera", buttonStyleBold);

            var activeVirtualCam = GetActiveVirtualCamera();

            if (editMode) {
                if (oldLookAt == null &&
                    oldFollow == null) {
                    SaveCamera(activeVirtualCam);
                }
                ChangeCamera(activeVirtualCam);

                GUILayout.BeginVertical();

                GUILayout.Label("Save to:");

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Enter rotation", buttonStyleBold)) {
                    var camRotation = activeVirtualCam.transform.rotation;

                    serializedProperty.rotateEnter = camRotation.eulerAngles;
                }

                if (GUILayout.Button("Exit rotation", buttonStyleBold)) {
                    var camRotation = activeVirtualCam.transform.rotation;

                    serializedProperty.rotateExit = camRotation.eulerAngles;
                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
    }

    private void ChangeCamera(CinemachineVirtualCamera activeVirtualCam) {
        var transform = serializedProperty.transform;
        activeVirtualCam.LookAt = transform;
        activeVirtualCam.Follow = transform;
    }

    private void SaveCamera(CinemachineVirtualCamera activeVirtualCam) {
        oldLookAt = activeVirtualCam.LookAt;
        oldFollow = activeVirtualCam.Follow;
        oldRotation = activeVirtualCam.transform.rotation;
    }

    private void RestoreCamera(CinemachineVirtualCamera activeVirtualCam) {
        activeVirtualCam.LookAt = oldLookAt;
        activeVirtualCam.Follow = oldFollow;
        activeVirtualCam.transform.rotation = oldRotation;

        oldLookAt = oldFollow = null;
        oldRotation = Quaternion.identity;
    }
}
