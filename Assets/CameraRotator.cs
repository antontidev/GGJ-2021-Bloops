using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MyBox;

public class CameraRotator : MonoBehaviour {
    [ConditionalField(nameof(enterState), false, State.Specific)]
    public Vector3 rotateEnter;

    [ConditionalField(nameof(exitState), false, State.Specific)]
    public Vector3 rotateExit;

    public float duration;

    private static Vector3 defaultRotation = new Vector3(45, 135, 0);

    public enum State {
        Default,
        Previous,
        Specific
    }

    public enum Events {
        Enter,
        Exit,
        Both
    }

    public Events events;

    [ConditionalField(nameof(events), true, Events.Enter)]
    public State exitState;

    [ConditionalField(nameof(events), true, Events.Exit)]
    public State enterState;

    public bool IsEnter {
        get {
            return events == Events.Enter ||
                   events == Events.Both;
        }
    }

    public bool IsExit {
        get {
            return events == Events.Exit ||
                   events == Events.Both;
        }
    }

#if UNITY_EDITOR 
    private new BoxCollider collider;
#endif

    private Dictionary<int, Quaternion> oldRotations;

    private void Start() {
        oldRotations = new Dictionary<int, Quaternion>();
    }

    /// <summary>
    /// Apply rotation what contains inside CameraRotator object
    /// </summary>
    /// <param name="vCam"></param>
    public void ApplyRotation(CinemachineVirtualCamera vCam) {
        var hash = vCam.GetHashCode();

        if (!oldRotations.ContainsKey(hash)) {
            var rotation = vCam.transform.rotation;

            oldRotations.Add(hash, rotation);
        }

        Rotate(vCam, enterState, rotateEnter);
    }

    /// <summary>
    /// Restores camera rotation when specific event meet
    /// </summary>
    /// <param name="vCam"></param>
    public void RestoreRotation(CinemachineVirtualCamera vCam) {
        Rotate(vCam, exitState, rotateExit);
    }

    private void Rotate(CinemachineVirtualCamera vCam, State state, Vector3 rotation) {
        if (state == State.Specific) {
            vCam.transform.DORotate(rotateEnter, duration);
        }
        else if (state == State.Default) {
            vCam.transform.DORotate(defaultRotation, duration);
        }
        else {
            var previousRotation = GetPreviousRotation(vCam);

            vCam.transform.DORotate(previousRotation, duration);
        }
    }

    private Vector3 GetPreviousRotation(CinemachineVirtualCamera vCam) {
        var hash = vCam.GetHashCode();

        if (oldRotations.ContainsKey(hash)) {
            var rotation = oldRotations[hash];

            return rotation.eulerAngles;
        }

        Debug.Log("There is no previousRotation for this camera");

        return defaultRotation;
    }

    // Bugy code
    private void OnDrawGizmos() {
        if (collider == null) {
            collider = GetComponent<BoxCollider>();
        }
        Gizmos.color = Color.green;
        var newSize = transform.localRotation * collider.size;
        var newCenter = transform.TransformPoint(collider.center);

        Gizmos.DrawWireCube(newCenter, newSize);
    }
}
