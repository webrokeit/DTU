using System;
using System.IO;
using Assets.Scripts.Lib;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(GUIText))]
public class ScaleText : MonoBehaviour {
    public int BaseScreenWidth = 1024;
    public int BaseScreenHeight = 768;

    [Range(1, 1250)]
    public int DesiredFontSize = 12;

    private int _prevDesiredFontSize;
    private float _minRatio = -1.0f;

	// Use this for initialization
    void Start() {
        CheckScaling();
    }
	
	// Update is called once per frame
	void Update () {
	    CheckScaling();
	}

    void CheckScaling() {
        var newMinRatio = Mathf.Min((float)Screen.width / BaseScreenWidth, (float)Screen.height / BaseScreenHeight);
        if (Math.Abs(newMinRatio - _minRatio) > Mathf.Epsilon || DesiredFontSize != _prevDesiredFontSize) {
            var newFontSize = Mathf.Max((int)(DesiredFontSize * newMinRatio), 1);
            guiText.fontSize = newFontSize;
            _minRatio = newMinRatio;
            _prevDesiredFontSize = DesiredFontSize;
        }
    }
}
