using Zenject;
using UnityEngine.SceneManagement;


using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
/*
public class QuickSceneSwitcher : EditorWindow {
    private QuickSceneSwitcherData currentConfig = null;

    private List<SceneAsset> scenes = null;

    private bool localProjectConfig = false;

    private int currentPickerWindow;

    private bool isSwitcherDataPicker {
        get {
            return Event.current.commandName == "ObjectSelectorUpdated" && 
                   EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow;
        }
    }

    private bool editMode = false;

    private Vector2 scrollPosition = Vector2.zero;

    // Prefix used to store preferences
    // Format is: SCENE_SWITCHER/DefaultCompany/ProjectName/
    private string PREFS_PREFIX;

    // Initialization
    void OnEnable() {
        // Use path.combine for better multiplatoform slash compability
        PREFS_PREFIX = Path.Combine($"QUICK_SCENE_SWITCHER", Application.companyName, Application.productName);

        scenes = new List<SceneAsset>();
    }

    private void OnDisable() {
        SaveConfig();
    }

    [MenuItem("Window/Quick Scene Switcher")]
    public static void ShowWindow() {
        GetWindow<QuickSceneSwitcher>("Scene Switcher");
    }

    private void OnGUI() {
        DrawHeader();

        for (int i = 0; i < scenes.Count; i++) {
            DrawScene(i);
        }

        DrawEditMode();

        GUILayout.EndScrollView();

        if (GUI.changed) {
            SaveConfig();
        }
    }

    private void DrawEditMode() {
        if (editMode) // 'EditMode' bottom options
        {
            GUILayout.Space(5);

            if (GUILayout.Button("Add Scene")) {
                AddScene();
            }

            GUILayout.Space(1);
        }
    }

    private void DrawScene(int i) {
        if (editMode) {
            GUILayout.BeginHorizontal();

            // Remove scene button
            bool sceneRemoved = false;
            if (GUILayout.Button("-")) {
                RemoveScene(i);
                sceneRemoved = true;
            }

            if (!sceneRemoved) {
                // Move scene up and down buttons
                if (GUILayout.Button("↑"))
                    MoveScene(i, -1);
                if (GUILayout.Button("↓"))
                    MoveScene(i, 1);

                // Scene asset field
                scenes[i] = EditorGUILayout.ObjectField(scenes[i], typeof(SceneAsset), false) as SceneAsset;
                // Scene button color field
                // buttonColors[i] = EditorGUILayout.ColorField(buttonColors[i], heightLayout, GUILayout.MaxWidth(80));
            }

            GUILayout.EndHorizontal();
        }
        else // if NOT in 'EditMode'
        {
            // Disable button if it corresponds to the currently open scene
            bool isCurrentScene = IsCurrentScene(scenes[i]);
            if (isCurrentScene)
                EditorGUI.BeginDisabledGroup(true);

            // Draw scene button
            if (scenes[i] != null && GUILayout.Button($"{scenes[i].name}{(isCurrentScene ? " (current)" : "")}"))
                OpenScene(scenes[i]);

            EditorGUI.EndDisabledGroup();
        }
    }

    private void DrawHeader() {
        // Scrollbar handling
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Label and 'EditMode' Button
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        GUILayout.Label("Quick Scenes");
        GUILayout.Label(currentConfig == null ? "None" : currentConfig.name);

        if (GUILayout.Button("Reload config")) {
            LoadConfig();
        }

        GUILayout.FlexibleSpace();

        localProjectConfig = EditorGUILayout.Toggle("Use local config", localProjectConfig);

        if (localProjectConfig) {
            if (isSwitcherDataPicker) {
                var newCurrentConfig = (QuickSceneSwitcherData)EditorGUIUtility.GetObjectPickerObject();

                if (currentConfig != null) {
                    SaveConfig();
                }

                if (newCurrentConfig != null && 
                    newCurrentConfig != currentConfig) {
                    currentConfig = newCurrentConfig;
                    LoadConfig();
                }

                currentPickerWindow = -1;
            }

            if (GUILayout.Button("Pick config")) {
                ShowPicker();
            }
        }

        editMode = GUILayout.Toggle(editMode, editMode ? " Exit Edit Mode " : " Edit Mode ");

        GUILayout.EndHorizontal();
        GUILayout.Space(3);
    }

    // Closes the current scene and opens the specified scene
    private void OpenScene(SceneAsset scene) {
        string scenePath = AssetDatabase.GetAssetPath(scene);
        SaveConfig();
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
    }

    private void AddScene() {
        scenes.Add(null);
    }
    private void MoveScene(int index, int increment) {
        // Make sure the new index is not out of range
        int newIndex = Mathf.Clamp(index + increment, 0, scenes.Count - 1);
        if (newIndex != index) {
            SceneAsset scene = scenes[index];
            RemoveScene(index);

            scenes.Insert(newIndex, scene);
        }
        SaveConfig();
    }
    private bool IsCurrentScene(SceneAsset scene) {
        return EditorSceneManager.GetActiveScene().path == AssetDatabase.GetAssetPath(scene);
    }

    private void RemoveScene(int index) {
        scenes.RemoveAt(index);
        SaveConfig();
    }

    private void ShowPicker() {
        currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;

        //use the ID you just created
        EditorGUIUtility.ShowObjectPicker<QuickSceneSwitcherData>(null, false, "", currentPickerWindow);
    }

    private void SaveConfig() {
        if (localProjectConfig && currentConfig != null) {
            currentConfig.scenes.Clear();
            currentConfig.buttonColors.Clear();

            for (int i = 0; i < scenes.Count; i++) {
                if (scenes[i] != null) {
                    var assetPath = AssetDatabase.GetAssetPath(scenes[i]);
                    currentConfig.scenes.Add(assetPath);
                }
            }

            currentConfig.localProjectConfig = localProjectConfig;
        }
        else {
            // Scenes Count
            EditorPrefs.SetInt(PREFS_PREFIX + "ScenesCount", scenes.Count);

            for (int i = 0; i < scenes.Count; i++) {
                // Scenes
                EditorPrefs.SetString(PREFS_PREFIX + $"Scene_{i}", AssetDatabase.GetAssetPath(scenes[i]));
            }

            // Edit Mode
            EditorPrefs.SetBool(PREFS_PREFIX + $"EditMode", editMode);
        }
    }

    private void LoadConfig() {
        scenes.Clear();
        localProjectConfig = false;

        if (currentConfig != null && currentConfig.localProjectConfig) {
            localProjectConfig = true;

            for (int i = 0; i < currentConfig.scenes.Count; i++) {
                var sceneName = currentConfig.scenes[i];

                AddScene();
                scenes[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneName);
            }
        }
        else {
            // Scenes Count
            int scenesCount = EditorPrefs.GetInt(PREFS_PREFIX + "ScenesCount");

            for (int i = 0; i < scenesCount; i++) {
                // Scenes
                AddScene();
                string scenePath = EditorPrefs.GetString(PREFS_PREFIX + $"Scene_{i}");
                scenes[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }

            // Edit Mode
            editMode = EditorPrefs.GetBool(PREFS_PREFIX + $"EditMode");
        }
    }
}*/
public class LevelNames {

    public static string Prelude {
        get {
            return "prelude";
        }
    }
    public static string Shared {
        get {
            return "Shared";
        }
    }
    public static string Level1 {
        get {
            return "level1";
        }
    }
}

/// <summary>
/// For accessing info about previous scene
/// </summary>
public class LevelLoader
{
    public static TimeManager timeManager;

    private static PreviousSceneGameSettings settings;

    public LevelLoader(PreviousSceneGameSettings _settings) {
        settings = _settings;
    }

    public static void LoadSceneWithoutNameCache(string sceneName, LoadSceneMode mode) {
        SceneManager.LoadScene(sceneName, mode);
    }

    public static void LoadScene(string sceneName, LoadSceneMode mode) {
        var activeScene = SceneManager.GetActiveScene();

        settings.previousSceneBuildIndex = activeScene.buildIndex;
        settings.previousSceneName = activeScene.name;
        settings.currentTime = timeManager.currentTime;

        SceneManager.LoadScene(sceneName, mode);
    }

    public static void HardReload() {
        var activeScene = SceneManager.GetActiveScene();

        settings.previousSceneBuildIndex = 0;
        settings.previousSceneName = "";

        SceneManager.LoadScene(activeScene.name, LoadSceneMode.Single);
    }

    public static bool IsStartLevel() {
        var activeScene = SceneManager.GetActiveScene();

        return settings.previousSceneName == "" && activeScene.name == LevelNames.Prelude;
    }
}
