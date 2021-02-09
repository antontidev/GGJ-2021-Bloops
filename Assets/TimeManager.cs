using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Day duration in real minutes")]
    private float dayDuration;

    /// <summary>
    /// Don't set this by yourself time in percents
    /// </summary>
    /// <remarks>
    /// [0, 1] range values
    /// </remarks>
    [ReadOnly]
    public float currentTimeNormalized;

    [ReadOnly]
    public float currentTime;

    [SerializeField] ShadowCascadesOption m_ShadowCascades = ShadowCascadesOption.NoCascades;

    public bool IsNight {
        get {
            return currentTimeNormalized > 0.5f;
        }
    }

    public bool IsDay {
        get {
            return !IsNight;
        }
    }

    public void MakeEvening() {
        dayDuration *= 10;

        var dayDurationInSeconds = dayDuration * 60;

        currentTime = dayDurationInSeconds - (dayDurationInSeconds * 0.65f);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > dayDuration * 60) {
            currentTime = 0;
        }

        currentTimeNormalized = currentTime / (dayDuration * 60);
    }
}
