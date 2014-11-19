using System.Collections;
using UnityEngine;

internal class KinectLoader : MonoBehaviour {
    public int LevelIndexToLoad = 1;
    private bool _kinectAvailable = false;
    private bool loadInitiated = false;

    private float _time;

    void Start() {
        _time = Time.time;
    }

    void Update() {
        if (_kinectAvailable || (Time.time - _time) < 5.0f) return;

        if (!loadInitiated) {
            StartCoroutine(LoadLevel());
            loadInitiated = true;
        }

        var manager = KinectManager.Instance;
        if (manager) {
            Debug.Log("Manager available");
        }
        if (!manager || !manager.IsInitialized()) return;

        Debug.Log("Kinect available");
        _kinectAvailable = true;
        Application.LoadLevel(LevelIndexToLoad);
    }

    IEnumerator LoadLevel() {
        Application.LoadLevel(LevelIndexToLoad);
        _kinectAvailable = true;
        yield return null;
    }
}
