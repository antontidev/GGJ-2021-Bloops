using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quick Scene Switcher/Create Config")]
[System.Serializable]
public class QuickSceneSwitcherData : ScriptableObject {
    public List<string> scenes;

    public bool localProjectConfig = false;

    public bool IsEmpty() {
        return scenes.Count == 0;
    }
}
