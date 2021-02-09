using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/*
public class QuickSceneSwitcher : EditorWindow {
    private QuickSceneSwitcherData currentConfig = null;

    private List<SceneAsset> scenes = null;

    private bool localProjectConfig = false;

    private int currentPickerWindow;
    private bool editMode = false;

    private Vector2 scrollPosition = Vector2.zero;
    private GUILayoutOption heightLayout;
    private Color darkGray;
    private Color footerColor;

    // Prefix used to store preferences
    // Format is: SCENE_SWITCHER/DefaultCompany/ProjectName/
    private string PREFS_PREFIX;

    // Initialization
    void OnEnable() {
        // Use path.combine for better multiplatoform slash compability
        PREFS_PREFIX = Path.Combine($"QUICK_SCENE_SWITCHER", Application.companyName, Application.productName);

        scenes = new List<SceneAsset>();

        // Style vars
        heightLayout = GUILayout.Height(22);
        darkGray = new Color(0.28f, 0.28f, 0.28f);
        footerColor = new Color(0, 0, 0, 0.25f);
    }

    [MenuItem("Window/Quick Scene Switcher")]
    public static void ShowWindow() {
        GetWindow<QuickSceneSwitcher>("Scene Switcher");
    }

    void OnGUI() {
        GUIStyle buttonStyleBold;
        Color previousColor;
        DrawHeader(out buttonStyleBold, out previousColor);

        // Draw each scene button (or each scene options if in 'EditMode')
        for (int i = 0; i < scenes.Count; i++) {
            DrawScene(buttonStyleBold, previousColor, i);
        }

        DrawEditMode(buttonStyleBold, previousColor);

        DrawFooter();

        if (GUI.changed) {
            SaveConfig();
        }
    }

    private void DrawEditMode(GUIStyle buttonStyleBold, Color previousColor) {
        if (editMode) // 'EditMode' bottom options
        {
            GUILayout.Space(5);
            GUI.backgroundColor = darkGray;
            UpdateButtonStyleTextColor(GUI.backgroundColor, buttonStyleBold);
            float previousHeight = buttonStyleBold.fixedHeight;
            buttonStyleBold.fixedHeight = 25;

            if (GUILayout.Button("Add Scene", buttonStyleBold))
                AddScene();

            GUILayout.Space(1);

            buttonStyleBold.fixedHeight = previousHeight;
            GUI.backgroundColor = previousColor;
            UpdateButtonStyleTextColor(GUI.backgroundColor, buttonStyleBold);

            if (GUILayout.Button("Load From BuildSettings", buttonStyleBold))
                LoadScenesFromBuildSettings();
        }
        else {
            // If NOT in 'EditMode' and there are no scenes buttons to show,
            // display an info message about how to add them
            if (scenes.Count == 0 || !(scenes.Any(i => i != null))) {
                GUILayout.Space(2);
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button($"Enter 'EditMode' and add scenes\nfor them to appear here", GUILayout.Height(50), GUILayout.MinWidth(10));
                EditorGUI.EndDisabledGroup();
            }
        }
    }

    private void DrawFooter() {
        // Draw Footer
        GUILayout.FlexibleSpace();
        GUIStyle footerStyle = EditorStyles.centeredGreyMiniLabel;
        footerStyle.normal.textColor = footerColor;
        GUILayout.Label("Made by Kelvip", footerStyle);
        GUILayout.Space(5);

        GUILayout.EndScrollView();
    }


    private void DrawHeader(out GUIStyle buttonStyleBold, out Color previousColor) {
        // Buttons Style
        buttonStyleBold = new GUIStyle(GUI.skin.button);
        buttonStyleBold.fixedHeight = 22;
        buttonStyleBold.fontStyle = FontStyle.Bold;

        // Title label style
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fixedHeight = 22;
        labelStyle.fontSize = 14;

        previousColor = GUI.contentColor;

        // Scrollbar handling
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Label and 'EditMode' Button
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        GUILayout.Label(" Quick Scenes", labelStyle);
        GUILayout.Label(currentConfig == null ? "None" : currentConfig.name, labelStyle);
        GUI.backgroundColor = editMode ? Color.white * 1.4f : darkGray;
        UpdateButtonStyleTextColor(GUI.backgroundColor, buttonStyleBold);

        if (GUILayout.Button("Reload config", buttonStyleBold)) {
            LoadConfig();
        }

        GUILayout.FlexibleSpace();

        localProjectConfig = EditorGUILayout.Toggle("Use local config", localProjectConfig);

        if (localProjectConfig) {
            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow) {
                if (currentConfig != null) {
                    SaveConfig();
                }
                currentConfig = (QuickSceneSwitcherData)EditorGUIUtility.GetObjectPickerObject();

                if (currentConfig != null) {
                    LoadConfig();
                }

                currentPickerWindow = -1;
            }

            if (GUILayout.Button("Pick config", buttonStyleBold)) {
                currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;

                //use the ID you just created
                EditorGUIUtility.ShowObjectPicker<QuickSceneSwitcherData>(null, false, "", currentPickerWindow);
            }
        }

        editMode = GUILayout.Toggle(editMode, editMode ? " Exit Edit Mode " : " Edit Mode ", buttonStyleBold);
        GUI.backgroundColor = previousColor;

        GUILayout.EndHorizontal();
        GUILayout.Space(3);
    }

    private void DrawScene(GUIStyle buttonStyleBold, Color previousColor, int i) {
        if (editMode) {
            GUILayout.BeginHorizontal();

            // Remove scene button
            bool sceneRemoved = false;
            if (GUILayout.Button("-", heightLayout, GUILayout.MaxWidth(25))) {
                RemoveScene(i);
                sceneRemoved = true;
            }

            if (!sceneRemoved) {
                // Move scene up and down buttons
                if (GUILayout.Button("↑", heightLayout, GUILayout.MaxWidth(25)))
                    MoveScene(i, -1);
                if (GUILayout.Button("↓", heightLayout, GUILayout.MaxWidth(25)))
                    MoveScene(i, 1);

                // Scene asset field
                scenes[i] = EditorGUILayout.ObjectField(scenes[i], typeof(SceneAsset), false, heightLayout) as SceneAsset;
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
            if (scenes[i] != null && GUILayout.Button($"{scenes[i].name}{(isCurrentScene ? " (current)" : "")}", buttonStyleBold))
                OpenScene(scenes[i]);

            GUI.backgroundColor = previousColor;
            EditorGUI.EndDisabledGroup();
        }
    }

    private void OnDisable() {
        SaveConfig();
    }

    #region SceneManagement

    // Closes the current scene and opens the specified scene
    private void OpenScene(SceneAsset scene) {
        string scenePath = AssetDatabase.GetAssetPath(scene);

        // Check if there are unsaved changes in the current scene and
        // ask if the user wants to save them before switching to the new scene
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(scenePath);
        SaveConfig();
    }

    // Add another scene slot
    private void AddScene() {
        scenes.Add(null);
    }

    // Remove the specified scene slot
    private void RemoveScene(int index) {
        scenes.RemoveAt(index);
        SaveConfig();
    }

    // Move the specified scene up or down on the list based on the increment -1[UP] +1[DOWN]
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

    // Returns true if 'scene' is the scene currently open
    private bool IsCurrentScene(SceneAsset scene) {
        return EditorSceneManager.GetActiveScene().path == AssetDatabase.GetAssetPath(scene);
    }

    // Loads all the scenes from the build settings (only if they are not already added)
    private void LoadScenesFromBuildSettings() {
        foreach (var scene in EditorBuildSettings.scenes) {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);

            if (!scenes.Contains(sceneAsset)) {
                int index;
                if (scenes.Contains(null))
                    index = scenes.IndexOf(null);
                else {
                    AddScene();
                    index = scenes.Count - 1;
                }

                scenes[index] = sceneAsset;
            }
        }
    }

    #endregion

    // This function chooses black or white for the scene button texts based on the color of the button
    private void UpdateButtonStyleTextColor(Color color, GUIStyle style) {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        bool whiteText = (v < 0.7f || (s > 0.4f && (h < 0.1f || h > 0.55f)));
        Color textColor = whiteText ? Color.white : Color.black;
        Color hoverTextColor = textColor + (whiteText ? Color.black * -0.2f : Color.white * 0.2f);

        style.normal.textColor = textColor;
        style.hover.textColor = hoverTextColor;
        style.focused.textColor = textColor;
        style.active.textColor = textColor;
    }

    #region PersistentData

    // Save current preferences to persistent data
    private void SaveConfig() {
        if (localProjectConfig && currentConfig != null) {
            currentConfig.scenes.Clear();

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

    // Load and parse preferences from persistent saved data
    private void LoadConfig() {
        scenes.Clear();

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

    #endregion
}*/