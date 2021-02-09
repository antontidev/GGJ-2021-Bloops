using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using Zenject;

public interface IPillowSeeker {
    void OnWheresPillow();

    void OnStartCutsceneEnd();

    void OnBackHomeCutsceneEnd();
}

public abstract class BaseGameManager : MonoBehaviour {
    [Inject]
    protected PreviousSceneGameSettings settings;

    protected SettingsManager settingsManager;

    public TimeManager timeManager;

    [SerializeField]
    protected WorldInteractor worldInteractor; 

    [SerializeField]
    private bool showDebugInfo;

    [SerializeField]
    private AudioEvent backgroundEvent;

    [SerializeField]
    private AudioSource audioSource;

    // Start is called before the first frame update
    public virtual void Start() {
        if (showDebugInfo) {
            LevelLoader.LoadSceneWithoutNameCache(LevelNames.Shared, LoadSceneMode.Additive);
        }

        settingsManager = GetComponent<SettingsManager>();

        timeManager.currentTime = settings.currentTime;
    }

#if UNITY_STANDALONE_WIN
    private void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }
#endif

    /// <summary>
    /// Invokes when player meets level switcher
    /// </summary>
    /// <param name="levelName"></param>
    public void OnLevelSwitch(LevelSwitcher switcher) {
        if (switcher.switchType == LevelSwitcher.SwitchType.ByName) {
            var levelName = switcher.sceneName;

            LevelLoader.LoadScene(levelName, LoadSceneMode.Single);
        }
        else {
        
        }
    }

    public void OnCameraRotatorEnter(GameObject rotator) {
        var camRotator = rotator.GetComponent<CameraRotator>();

        if (camRotator.IsEnter) {
            var vCam = settingsManager.vCam;

            camRotator.ApplyRotation(vCam);
        }
    }

    public void OnCameraRotatorExit(GameObject rotator) {
        var camRotator = rotator.GetComponent<CameraRotator>();

        if (camRotator.IsExit) {
            var vCam = settingsManager.vCam;

            camRotator.RestoreRotation(vCam);
        }
    }

    public void PlayBackgroundMusic() {
        backgroundEvent.Play(audioSource);
    }

    /// <summary>
    /// Used to erase previous level info
    /// </summary>
    private void OnApplicationQuit() {
        settings.previousSceneBuildIndex = -1;
        settings.previousSceneName = "";

        settings.currentTime = timeManager.currentTime;
    }
}

public class GameManager : BaseGameManager, IPillowSeeker {
    [SerializeField]
    private PlayableDirector director;

    [Header("Timeline assets")]
    [SerializeField]
    private TimelineAsset startGamePlayable;

    [SerializeField]
    private TimelineAsset fromLevelPlayable;

    public override void Start() {
        base.Start();

        if (LevelLoader.IsStartLevel()) {
            director.playableAsset = startGamePlayable;

            director.Play();
        }
        else {
            director.playableAsset = fromLevelPlayable;

            director.Play();
        }

        settingsManager.ApplyCinemachineSettings();
    }

    /// <summary>
    /// Invokes when player stands in front of bed
    /// </summary>
    public void OnWheresPillow() {
    }

    /// <summary>
    /// Invokes when cat returns to home
    /// </summary>
    public void OnBackHomeCutsceneEnd() {
        LevelLoader.HardReload();
    }

    public void OnStartCutsceneEnd() {
        settingsManager.ApplyJoystickSettings();
    }
}
