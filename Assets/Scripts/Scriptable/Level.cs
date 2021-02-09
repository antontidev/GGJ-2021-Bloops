using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MyBox;

[CreateAssetMenu(menuName ="Levels/Level")]
public class Level : ScriptableObject
{
    public SceneAsset scene;

    [ReadOnly]
    public bool IsOpen;
}
