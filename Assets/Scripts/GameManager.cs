using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    private bool showDebugInfo;

    // Start is called before the first frame update
    void Start() {
        if (showDebugInfo) {
            SceneManager.LoadScene("Shared", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
