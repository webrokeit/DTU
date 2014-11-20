using System;
using System.Collections;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(KinectManager))]
internal class KinectLoader : MonoBehaviour {
    public int LevelIndexToLoad = 1;

    private float _time;

    void Start() {
        _time = Time.time;
    }

    void Update() {
        if ((Time.time - _time) < 0.1f) return;
        Application.LoadLevel(LevelIndexToLoad);
    }
}
