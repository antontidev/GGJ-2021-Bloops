using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class LevelSwitcher : MonoBehaviour
{
    [ConditionalField(nameof(switchType), false, SwitchType.ByName)]
    public string sceneName;

    public enum SwitchType {
        ByName,
        FromMenu
    }

    public SwitchType switchType;
}
