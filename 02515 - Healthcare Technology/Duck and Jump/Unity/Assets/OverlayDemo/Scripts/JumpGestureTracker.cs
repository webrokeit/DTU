using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class JumpGestureTracker : MonoBehaviour {
    public const int LeftKneeIndex = (int)KinectWrapper.SkeletonJoint.LEFT_KNEE;
    public const int RightKneeIndex = (int)KinectWrapper.SkeletonJoint.RIGHT_KNEE;
    public const int HipIndex = (int)KinectWrapper.SkeletonJoint.HIPS;

    public GameObject LeftKneeOverlayObject;
    public GameObject RightKneeOverlayObject;
    public GameObject HipOverlayObject;

    public float SmoothFactor = 5f;
    public bool DisplayOverlays = false;
    private bool _displayState;

    public GUIText DebugText;

    private readonly MinMax _minMaxLeft = new MinMax();
    private readonly MinMax _minMaxRight = new MinMax();
    private readonly MinMax _minMaxHip = new MinMax();
    
    private float _distanceToCameraLeftKnee = 10f;
    private float _distanceToCameraRightKnee = 10f;
    private float _distanceToCameraHip = 10f;

	// Use this for initialization
	void Start () {
        if (LeftKneeOverlayObject) {
            LeftKneeOverlayObject.SetActive(true);
            _distanceToCameraLeftKnee = (LeftKneeOverlayObject.transform.position - Camera.main.transform.position).magnitude;
        }
        if (RightKneeOverlayObject) {
            RightKneeOverlayObject.SetActive(true);
            _distanceToCameraRightKnee = (RightKneeOverlayObject.transform.position - Camera.main.transform.position).magnitude;
        }
        if (HipOverlayObject) {
            HipOverlayObject.SetActive(true);
	        _distanceToCameraHip = (HipOverlayObject.transform.position - Camera.main.transform.position).magnitude;
	    }
	    _displayState = true;
	}
	
	// Update is called once per frame
	void Update () {
        var manager = KinectManager.Instance;

        if (manager && manager.IsInitialized()) {
            if (manager.IsUserDetected()) {
                var userId = manager.GetPlayer1ID();
                if (userId > 0 && AreJointsTracked(manager, userId)) {
                    var leftPos = manager.GetRawSkeletonJointPos(userId, LeftKneeIndex);
                    var rightPos = manager.GetRawSkeletonJointPos(userId, RightKneeIndex);
                    var hipPos = manager.GetRawSkeletonJointPos(userId, HipIndex);

                    _minMaxLeft.Update(leftPos.y);
                    _minMaxRight.Update(rightPos.y);
                    _minMaxHip.Update(hipPos.y);

                    if (AreNonZero(leftPos, rightPos, hipPos)) {
                        if (DebugText) {
                            DebugText.text = leftPos.y + " & " + rightPos.y + " & " + hipPos.y;
                        }

                        HandleDisplayOverlays(manager, leftPos, rightPos, hipPos);
                    }
                }
            } else {
                if (DebugText) {
                    DebugText.text = "(" + Math.Round(_minMaxLeft.Min, 4) + " ; " + Math.Round(_minMaxLeft.Max, 4) + ") " +
                                     "(" + Math.Round(_minMaxRight.Min, 4) + " ; " + Math.Round(_minMaxRight.Max, 4) + ") " +
                                     "(" + Math.Round(_minMaxHip.Min, 4) + " ; " + Math.Round(_minMaxHip.Max, 4) + ")";
                }
            }
        }
	}

    void HandleDisplayOverlays(KinectManager manager, Vector3 leftPos, Vector3 rightPos, Vector3 hipPos) {
        if (DisplayOverlays) {
            if (!_displayState) {
                if (LeftKneeOverlayObject) LeftKneeOverlayObject.SetActive(true);
                if (RightKneeOverlayObject) RightKneeOverlayObject.SetActive(true);
                if (HipOverlayObject) HipOverlayObject.SetActive(true);
                _displayState = true;
            }
            PositionOverlay(manager, leftPos, LeftKneeOverlayObject, _distanceToCameraLeftKnee);
            PositionOverlay(manager, rightPos, RightKneeOverlayObject, _distanceToCameraRightKnee);
            PositionOverlay(manager, hipPos, HipOverlayObject, _distanceToCameraHip);
        } else if (_displayState) {
            if (LeftKneeOverlayObject) LeftKneeOverlayObject.SetActive(false);
            if (RightKneeOverlayObject) RightKneeOverlayObject.SetActive(false);
            if (HipOverlayObject) HipOverlayObject.SetActive(false);
            _displayState = false;
        }
    }

    static bool AreJointsTracked(KinectManager manager, uint userId) {
        return manager.IsJointTracked(userId, LeftKneeIndex)
               && manager.IsJointTracked(userId, RightKneeIndex)
               && manager.IsJointTracked(userId, HipIndex);
    }

    static bool AreNonZero(params Vector3[] vectors) {
        return vectors != null && vectors.All(vector => vector != Vector3.zero);
    }

    void PositionOverlay(KinectManager manager, Vector3 posJoint, GameObject overlayObject, float distanceToCamera) {
        if (!overlayObject) return;
        
        // 3d position to depth
        var posDepth = manager.GetDepthMapPosForJointPos(posJoint);

        // depth pos to color pos
        var posColor = manager.GetColorMapPosForDepthPos(posDepth);

        var scaleX = posColor.x / KinectWrapper.Constants.ColorImageWidth;
        var scaleY = 1.0f - posColor.y / KinectWrapper.Constants.ColorImageHeight;

        var vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
        overlayObject.transform.position = Vector3.Lerp(overlayObject.transform.position, vPosOverlay, SmoothFactor * Time.deltaTime);
    }

    private class MinMax {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public MinMax() {
            Min = float.MaxValue;
            Max = float.MinValue;
        }

        public void Update(float val) {
            if (Min > val) Min = val;
            if (Max < val) Max = val;
        }
    }
}

public class JumpGestureState {
    public bool Complete { get; private set; }

    public float StartTime { get; private set; }
    public float StartLeftPos { get; private set; }
    public float StartRightPos { get; private set; }
    public float StartHipPos { get; private set; }

    private JumpGestureState() {
        Complete = false;
    }

    public JumpGestureState Create(float left, float right, float hip) {
        if (-0.4f > left || left > -0.2f) return null;
        if (-0.4f > right || left > -0.2f) return null;

        return new JumpGestureState {
            StartTime = Time.time,
            StartLeftPos = left,
            StartRightPos = right,
            StartHipPos = hip,
        };
    }

    // Start: -0.4 < left|right < -0.2
    // Up: -0.1 < left|right
}