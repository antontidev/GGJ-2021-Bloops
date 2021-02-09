using SukharevShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICatSound {
    void MeowSound();

    void FootSound();
}

public interface IPillowHolder {
    void HoldMyPillow(GameObject pillow);
}

[System.Serializable]
public class UntityEventLevelSwitcher : UnityEvent<LevelSwitcher> {
}

public class CatBehaviour : MonoBehaviour, ICatSound, IPillowHolder
{
    [SerializeField]
    private GameObject myPillow;

    [Header("Audio")]
    [SerializeField]
    private AudioEvent meowEvent;

    [SerializeField]
    private AudioEvent footEvent;

    [SerializeField]
    private AudioSource playerSource;

    [Header("Tags")]
    [SerializeField]
    private Tag levelSwitcher;

    [SerializeField]
    private Tag pillowTag;

    [SerializeField]
    private Tag pillowFallTag;

    [SerializeField]
    private Tag cameraRotator;

    [Header("Enter Events")]
    public UntityEventLevelSwitcher OnLevelSwitch;

    public UnityEventGameObject OnPillowReached;

    public UnityEventGameObject OnPillowFallReached;

    public UnityEventGameObject OnCameraRotatorEnter;

    [Header("Exit Events")]
    public UnityEventGameObject OnCameraRotatorExit;

    public void FootSound() {
    }

    public void MeowSound() {
        meowEvent.Play(playerSource);
    }

    private void OnTriggerEnter(Collider other) {
        var gameObj = other.gameObject;

        if (gameObj.HasTag(levelSwitcher)) {
            var switcher = gameObj.GetComponent<LevelSwitcher>();

            var sceneName = switcher.sceneName;

            OnLevelSwitch?.Invoke(switcher);
        }
        else if (gameObj.HasTag(pillowTag)) {
            OnPillowReached?.Invoke(gameObj);

            HoldMyPillow(gameObj);

            gameObj.SetActive(false);
        }
        else if (gameObj.HasTag(pillowFallTag)) {
            OnPillowFallReached?.Invoke(gameObj);

            meowEvent.Play(playerSource);
        }
        else if (gameObj.HasTag(cameraRotator)) {
            OnCameraRotatorEnter?.Invoke(gameObj);
        }
    }

    private void OnTriggerExit(Collider other) {
        var gameObj = other.gameObject;

        if (gameObj.HasTag(cameraRotator)) {
            OnCameraRotatorExit?.Invoke(gameObj);
        }
    }

    public void HoldMyPillow(GameObject pillow) {
        myPillow.SetActive(true);
    }
}
