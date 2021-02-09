using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManagerLevel : BaseGameManager {
    [SerializeField]
    private Animator crowAnimator;

    [SerializeField]
    private Rigidbody pillowBody;

    [SerializeField]
    private GameObject portal;

    public override void Start() {
        base.Start();

        settingsManager.ApplyAllSettings();

        PlayBackgroundMusic();
    }

    public void OnPillowFallReached(GameObject pillowFall) {
        pillowFall.SetActive(false);
        crowAnimator?.SetTrigger("away");
        pillowBody.useGravity = true;
        crowAnimator.gameObject.GetComponent<AudioSource>().Stop();
    }

    public void OnPillowReached(GameObject pillow) {
        portal.SetActive(true);
    }
}
