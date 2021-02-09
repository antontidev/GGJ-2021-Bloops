using MyBox;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/PreviousScene")]
public class PreviousSceneGameSettings : GameSettings {
    [ReadOnly]
    public string previousSceneName;

    [ReadOnly]
    public int previousSceneBuildIndex;

    public float currentTime;
}
