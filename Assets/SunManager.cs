using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;

    public Light sunLight;

    public AnimationCurve sunIntensity;

    public AnimationCurve sunRotationX;

    public AnimationCurve sunRotationY;

    // Update is called once per frame
    void Update()
    {
        var timeNormalized = timeManager.currentTimeNormalized;

        // Intencity
        var intensity = sunIntensity.Evaluate(timeNormalized);
        sunLight.intensity = intensity;

        // Rotation
        var lightGameObject = sunLight.gameObject;

        var xRotation = sunRotationX.Evaluate(timeNormalized);
        var yRotation = sunRotationY.Evaluate(timeNormalized);

        var rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        lightGameObject.transform.rotation = rotation;
    }
}
