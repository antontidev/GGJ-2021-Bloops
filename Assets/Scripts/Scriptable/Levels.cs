using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Level Manager")]
public class Levels : ScriptableObject {
    public List<Level> levels;

    [ReadOnly]
    public List<Level> openLevels;

    public void OpenLevel(Level level) {
        if (levels.Contains(level) && !openLevels.Contains(level)) {
            openLevels.Add(level);

            level.IsOpen = true;
        }
    }

}
