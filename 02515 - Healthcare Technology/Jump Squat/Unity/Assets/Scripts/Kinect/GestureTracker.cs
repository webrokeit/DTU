using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets.Scripts.Kinect;
using Assets.Scripts.Kinect.Gesture;
using Assets.Scripts.Lib;
using UnityEngine;
using System.Collections;

public class GestureTracker : MonoBehaviour {
    public bool DisplayOverlays = false;
    public JointOverlays Overlays;
    public float SmoothFactor = 5f;
    private bool _displayState;

    public Gestures[] GesturesToTrack = {};
    private IGesture[] _gestureTrackers = {};

    public bool DebugMode;


    public LogModes LogMode;
    public string DataFile = "C:/Logs/{app_name}/{session}_data.csv";
    private JointLogger _logger;

    public delegate void GestureDetectedEvent(uint userId, IGesture gesture);
    public event GestureDetectedEvent GestureDetected;

	// Use this for initialization
	void Start () {
	    (Overlays = Overlays ?? new JointOverlays()).Init();
        if(DebugMode) Debug.Log("Overlays: " + Overlays.Count());
        foreach (var overlay in Overlays.ToList()) {
            overlay.SetActive(DisplayOverlays);
            Overlays[overlay] = (overlay.transform.position - Camera.main.transform.position).magnitude;
        }
        _displayState = DisplayOverlays;

	    if (GesturesToTrack != null && GesturesToTrack.Length > 0) {
	        var gestureSet = (new HashSet<Gestures>(GesturesToTrack)).ToArray();
            _gestureTrackers = new IGesture[gestureSet.Length];
            for (var i = 0; i < gestureSet.Length; i++) {
                switch (gestureSet[i]) {
                    case Gestures.Squat:
                        _gestureTrackers[i] = new SquatGesture();
                        break;
                    case Gestures.Jump:
                        _gestureTrackers[i] = new JumpGesture();
                        break;
                    case Gestures.DoubleClap:
                        _gestureTrackers[i] = new DoubleClapGesture();
                        break;
                }
	        }
	    }

        if (LogMode > LogModes.None) {
            _logger = new JointLogger(DataFile);
        }
	}
	
	// Update is called once per frame
	void Update () {
        var manager = KinectManager.Instance;
        if (!manager || !manager.IsInitialized() || !manager.IsUserDetected()) return;

	    var userId = manager.GetPlayer1ID();
	    if (userId <= 0) return;

	    var positions = new JointPositions(manager, userId);
	    var gesturesDetected = new List<IGesture>();

	    foreach (var gestureTracker in _gestureTrackers.Where(gestureTracker => gestureTracker != null && gestureTracker.Update(positions))) {
	        gesturesDetected.Add(gestureTracker);
	        if (GestureDetected != null) {
	            GestureDetected(userId, gestureTracker);
	        }
	    }

	    HandleDisplayOverlays(manager, positions);
        
	    if (_logger != null && (LogMode == LogModes.EachFrame || (LogMode == LogModes.GesturesOnly && gesturesDetected.Count > 0))) {
	        _logger.Log(positions, userId, gesturesDetected);
	    }
	}

    void OnApplicationQuit() {
        if (_logger != null) {
            _logger.Dispose();
        }
    }

    void HandleDisplayOverlays(KinectManager manager, JointPositions positions) {
        if (DisplayOverlays) {
            if (!_displayState) {
                foreach (var overlay in Overlays) {
                    overlay.SetActive(true);
                }
                _displayState = true;
            }
            if(positions.AreAllNonZero(Overlays.GetOverlayedJoints())){
                foreach (var overlay in Overlays) {
                    Overlays.Position(manager, positions, overlay, SmoothFactor);
                }
            }
        } else if (_displayState) {
            foreach (var overlay in Overlays) {
                overlay.SetActive(false);
            }
            _displayState = false;
        }
    }
}
