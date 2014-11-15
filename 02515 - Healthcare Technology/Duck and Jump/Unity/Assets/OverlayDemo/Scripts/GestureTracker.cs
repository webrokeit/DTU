using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
    public GUIText DebugText;
    public GUIText GestureText;
    
    private float _gestureDisplay;

	// Use this for initialization
	void Start () {
	    (Overlays = Overlays ?? new JointOverlays()).Init();
        if(DebugMode) Debug.Log("Overlays: " + Overlays.Count());
        foreach (var overlay in Overlays.ToList()) {
            overlay.SetActive(true);
            Overlays[overlay] = (overlay.transform.position - Camera.main.transform.position).magnitude;
        }
        _displayState = true;

        GestureText.text = string.Empty;

	    if (GesturesToTrack != null && GesturesToTrack.Length > 0) {
	        var gestureSet = (new HashSet<Gestures>(GesturesToTrack)).ToArray();
            _gestureTrackers = new IGesture[gestureSet.Length];
            for (var i = 0; i < gestureSet.Length; i++) {
                switch (gestureSet[i]) {
                    case Gestures.Duck:
                        _gestureTrackers[i] = new DuckGesture();
                        break;
                    case Gestures.Jump:
                        _gestureTrackers[i] = new JumpGesture();
                        break;
                }
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
	    ResetGestureRecognized();
        
        var manager = KinectManager.Instance;
        if (manager && manager.IsInitialized()) {
            if (manager.IsUserDetected()) {
                var userId = manager.GetPlayer1ID();
                if (userId > 0) {
                    var positions = new JointPositions(manager, userId);

                    foreach (var gestureTracker in _gestureTrackers) {
                        if (gestureTracker == null) continue;
                        if (gestureTracker.Update(positions)) {
                            GestureRecognized(gestureTracker);
                        }
                    }

                    HandleDisplayOverlays(manager, positions);
                }
            } else {
                if (DebugText) {
                    
                }
            }
        }
	}

    void GestureRecognized(IGesture gesture) {
        GestureText.text = gesture.Count + " " + gesture.Gesture.ToString().ToUpper() + (gesture.Count == 1 ? string.Empty : "S") + " RECOGNIZED!";
        _gestureDisplay = Time.time;
    }

    void ResetGestureRecognized() {
        if (Time.time - _gestureDisplay > 5.0f) {
            GestureText.text = string.Empty;
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
            foreach (var overlay in Overlays) {
                Overlays.Position(manager, positions, overlay, SmoothFactor);
            }
        } else if (_displayState) {
            foreach (var overlay in Overlays) {
                overlay.SetActive(false);
            }
            _displayState = false;
        }
    }
}

public interface IGesture {
    int Count { get; }
    Gestures Gesture { get; }
    bool Update(JointPositions positions);
}

public class JumpGesture : IGesture {
    public int Count { get; private set; }
    public Gestures Gesture { get { return Gestures.Jump; } }

    private readonly Queue<JumpGestureState> _states = new Queue<JumpGestureState>(1000);

    public bool Update(JointPositions positions) {
        var newState = JumpGestureState.Create(positions);
        if (newState != null) _states.Enqueue(newState);

        while (_states.Count > 0 && (Time.time - _states.Peek().StartTime > 1.5f || _states.Peek().Complete)) {
            _states.Dequeue();
        }

        if (_states.Any(state => state.Update(positions))) {
            _states.Clear();
            Count++;
            return true;
        }
        return false;
    }
}

public class DuckGesture : IGesture {
    public int Count { get; private set; }
    public Gestures Gesture { get { return Gestures.Duck; } }

    private readonly Queue<DuckGestureState> _states = new Queue<DuckGestureState>(1000);

    public bool Update(JointPositions positions) {
        var newState = DuckGestureState.Create(positions);
        if (newState != null)  _states.Enqueue(newState);

        while (_states.Count > 0 && (Time.time - _states.Peek().StartTime > 1.5f || _states.Peek().Complete)) {
            _states.Dequeue();
        }

        if (_states.Any(state => state.Update(positions))) {
            _states.Clear();
            Count++;
            return true;
        }
        return false;
    }
}

public class JumpGestureState {
    public bool Complete { get; private set; }
    public bool Leaped { get { return LeapedPositions != null; }}

    public float StartTime { get; private set; }
    public float LeapedTime { get; private set; }

    public JointPositions StartPositions { get; private set; }
    public JointPositions LeapedPositions { get; private set; }

    private static readonly Joints[] UsedJoints = { Joints.LeftKnee, Joints.RightKnee };

    private JumpGestureState() {
        Complete = false;
        LeapedTime = 0.000f;
    }

    public bool Update(JointPositions positions) {
        if (Complete) return Complete;
        positions.InitIfNotSet(UsedJoints);
        if (!positions.AreAllNonZero(UsedJoints)) return false;

        if (!Leaped) {
            if (positions[Joints.LeftKnee].y > StartPositions[Joints.LeftKnee].y
                && Mathf.Abs(positions[Joints.LeftKnee].y - StartPositions[Joints.LeftKnee].y) >= 0.3f
                && positions[Joints.RightKnee].y > StartPositions[Joints.RightKnee].y
                && Mathf.Abs(positions[Joints.RightKnee].y - StartPositions[Joints.RightKnee].y) >= 0.3f
            ) {
                LeapedTime = Time.time;
                LeapedPositions = positions;
            }
        } else {
            if (positions[Joints.LeftKnee].y < LeapedPositions[Joints.LeftKnee].y
                && Mathf.Abs(positions[Joints.LeftKnee].y - StartPositions[Joints.LeftKnee].y) < 0.05f
                && positions[Joints.RightKnee].y < LeapedPositions[Joints.RightKnee].y
                && Mathf.Abs(positions[Joints.RightKnee].y - StartPositions[Joints.RightKnee].y) < 0.05f
            ) {
                Complete = true;
            }
        }
        return Complete;
    }

    public static JumpGestureState Create(JointPositions positions) {
        positions.InitIfNotSet(UsedJoints);
        if (-0.4f > positions[Joints.LeftKnee].y || positions[Joints.LeftKnee].y > -0.2f) return null;
        if (-0.4f > positions[Joints.RightKnee].y || positions[Joints.RightKnee].y > -0.2f) return null;

        return new JumpGestureState {
            StartTime = Time.time,
            StartPositions = positions
        };
    }
}

public class DuckGestureState {
    public bool Complete { get; private set; }
    public bool Ducked { get { return DuckedPositions != null; } }

    public float StartTime { get; private set; }
    public float DuckedTime { get; private set; }

    public JointPositions StartPositions { get; private set; }
    public JointPositions DuckedPositions { get; private set; }

    private static readonly Joints[] UsedJoints = { Joints.LeftKnee, Joints.RightKnee, Joints.LeftHip, Joints.RightHip };

    private DuckGestureState() {
        Complete = false;
        DuckedTime = 0.000f;
    }

    public bool Update(JointPositions positions) {
        if (Complete) return Complete;
        positions.InitIfNotSet(UsedJoints);
        if (!positions.AreAllNonZero(UsedJoints)) return false;

        if (!Ducked) {
            if (positions[Joints.LeftKnee].y < StartPositions[Joints.LeftKnee].y 
                && Mathf.Abs(positions[Joints.LeftKnee].y - StartPositions[Joints.LeftKnee].y) >= 0.15f
                && positions[Joints.RightKnee].y < StartPositions[Joints.RightKnee].y
                && Mathf.Abs(positions[Joints.RightKnee].y - StartPositions[Joints.RightKnee].y) >= 0.15f
                && positions[Joints.LeftHip].y < StartPositions[Joints.LeftHip].y
                && Mathf.Abs(positions[Joints.LeftHip].y - StartPositions[Joints.LeftHip].y) >= 0.35f
                && positions[Joints.RightHip].y < StartPositions[Joints.RightHip].y
                && Mathf.Abs(positions[Joints.RightHip].y - StartPositions[Joints.RightHip].y) >= 0.35f
            ) {
                DuckedTime = Time.time;
                DuckedPositions = positions;
            }
        } else {
            if (positions[Joints.LeftKnee].y > DuckedPositions[Joints.LeftKnee].y 
                && Mathf.Abs(positions[Joints.LeftKnee].y - StartPositions[Joints.LeftKnee].y) < 0.05f
                && positions[Joints.RightKnee].y > DuckedPositions[Joints.RightKnee].y
                && Mathf.Abs(positions[Joints.RightKnee].y - StartPositions[Joints.RightKnee].y) < 0.05f
                && positions[Joints.LeftHip].y > DuckedPositions[Joints.LeftHip].y
                && Mathf.Abs(positions[Joints.LeftHip].y - StartPositions[Joints.LeftHip].y) < 0.05f
                && positions[Joints.RightHip].y > DuckedPositions[Joints.RightHip].y
                && Mathf.Abs(positions[Joints.RightHip].y - StartPositions[Joints.RightHip].y) < 0.05f
            ) {
                Complete = true;
            }
        }
        return Complete;
    }

    public static DuckGestureState Create(JointPositions positions) {
        positions.InitIfNotSet(UsedJoints);
        if (-0.4f > positions[Joints.LeftKnee].y || positions[Joints.LeftKnee].y > -0.2f) return null;
        if (-0.4f > positions[Joints.RightKnee].y || positions[Joints.RightKnee].y > -0.2f) return null;
        if (-0.1f > positions[Joints.LeftHip].y || positions[Joints.LeftHip].y > 0.1f) return null;
        if (-0.1f > positions[Joints.RightHip].y || positions[Joints.RightHip].y > 0.1f) return null;

        return new DuckGestureState {
            StartTime = Time.time,
            StartPositions = positions
        };
    }
}

public class JointPositions {
    public const int JointCount = (int)Joints.Count;
    private readonly Vector3[] _positions;
    private readonly bool[] _isSet;
    private readonly uint _userId;
    private readonly KinectManager _manager;

    public JointPositions(KinectManager manager, uint userId) {
        _positions = new Vector3[JointCount];
        _isSet = new bool[JointCount];
        _manager = manager;
        _userId = userId;
    }

    public Vector3 this[Joints joint] {
        get { return _positions[(int) joint]; }
        set {
            _isSet[(int) joint] = true;
            _positions[(int) joint] = value; 
        }
    }

    public bool IsSet(Joints joint) {
        return _isSet[(int) joint];
    }

    public void InitIfNotSet(params Joints[] jointsToCheck) {
        if (jointsToCheck == null || jointsToCheck.Length < 1) return;
        foreach (var joint in jointsToCheck.Where(joint => !IsSet(joint))) {
            this[joint] = _manager.GetRawSkeletonJointPos(_userId, (int) joint);
        }
    }

    public bool AreAllNonZero(params Joints[] jointsToCheck) {
        if (jointsToCheck == null || jointsToCheck.Length < 1) return false;
        return jointsToCheck.All(joint => this[joint] != Vector3.zero);
    }

    public bool AreJointsTracked(params Joints[] jointsToCheck) {
        if (jointsToCheck == null || jointsToCheck.Length < 1) return false;
        return jointsToCheck.All(joint => _manager.IsJointTracked(_userId, (int) joint));
    }
}

// Basically a rename of NuiSkeletonPositionIndex, DO NOT REORDER
public enum Joints {
    Hips = 0,
    Spine,
    Neck,
    Head,
    LeftShoulder,
    LeftElbow,
    LeftWrist,
    LeftHand,
    RightShoulder,
    RightElbow,
    RightWrist,
    RightHand,
    LeftHip,
    LeftKnee,
    LeftAnkle,
    LeftFoot,
    RightHip,
    RightKnee,
    RightAnkle,
    RightFoot,
    Count
}

public enum Gestures {
    Jump,
    Duck
}

[Serializable]
public class JointOverlays : IEnumerable<GameObject> {
    public GameObject Hips;
    public GameObject Spine;
    public GameObject Neck;
    public GameObject Head;
    public GameObject LeftShoulder;
    public GameObject LeftElbow;
    public GameObject LeftWrist;
    public GameObject LeftHand;
    public GameObject RightShoulder;
    public GameObject RightElbow;
    public GameObject RightWrist;
    public GameObject RightHand;
    public GameObject LeftHip;
    public GameObject LeftKnee;
    public GameObject LeftAnkle;
    public GameObject LeftFoot;
    public GameObject RightHip;
    public GameObject RightKnee;
    public GameObject RightAnkle;
    public GameObject RightFoot;

    private Dictionary<GameObject, float> _overlays;
    private Dictionary<GameObject, Joints> _mappings;

    public float this[GameObject overlay] {
        get { return _overlays[overlay]; }
        set { _overlays[overlay] = value; }
    }

    public void Init() {
        _overlays = (new[] {
            Hips, Spine, Neck, Head, LeftShoulder, LeftElbow, LeftWrist, LeftHand,
            RightShoulder, RightElbow, RightWrist, RightHand, LeftHip, LeftKnee,
            LeftAnkle, LeftFoot, RightHip, RightKnee, RightAnkle, RightFoot
        }).Where(obj => obj != null).ToDictionary(obj => obj, obj => 0f);

        _mappings = new Dictionary<GameObject, Joints>();
        if (Hips != null) _mappings[Hips] = Joints.Hips;
        if (Spine != null) _mappings[Spine] = Joints.Spine;
        if (Neck != null) _mappings[Neck] = Joints.Neck;
        if (Head != null) _mappings[Head] = Joints.Head;
        if (LeftShoulder != null) _mappings[LeftShoulder] = Joints.LeftShoulder;
        if (LeftElbow != null) _mappings[LeftElbow] = Joints.LeftElbow;
        if (LeftWrist != null) _mappings[LeftWrist] = Joints.LeftWrist;
        if (LeftHand != null) _mappings[LeftHand] = Joints.LeftHand;
        if (RightShoulder != null) _mappings[RightShoulder] = Joints.RightShoulder;
        if (RightElbow != null) _mappings[RightElbow] = Joints.RightElbow;
        if (RightWrist != null) _mappings[RightWrist] = Joints.RightWrist;
        if (RightHand != null) _mappings[RightHand] = Joints.RightHand;
        if (LeftHip != null) _mappings[LeftHip] = Joints.LeftHip;
        if (LeftKnee != null) _mappings[LeftKnee] = Joints.LeftKnee;
        if (LeftAnkle != null) _mappings[LeftAnkle] = Joints.LeftAnkle;
        if (LeftFoot != null) _mappings[LeftFoot] = Joints.LeftFoot;
        if (RightHip != null) _mappings[RightHip] = Joints.RightHip;
        if (RightKnee != null) _mappings[RightKnee] = Joints.RightKnee;
        if (RightAnkle != null) _mappings[RightAnkle] = Joints.RightAnkle;
        if (RightFoot != null) _mappings[RightFoot] = Joints.RightFoot;
    }

    public void Position(KinectManager manager, JointPositions positions, GameObject overlayObject, float smoothFactor) {
        if (!overlayObject || !_mappings.ContainsKey(overlayObject)) return;

        var joint = _mappings[overlayObject];
        positions.InitIfNotSet(joint);
        if (!positions.AreAllNonZero(joint)) return;

        var jointPosition = positions[joint];
        var distanceToCamera = _overlays[overlayObject];

        var depthPosition = manager.GetDepthMapPosForJointPos(jointPosition);
        var colorPosition = manager.GetColorMapPosForDepthPos(depthPosition);

        var scaleX = colorPosition.x / KinectWrapper.Constants.ColorImageWidth;
        var scaleY = 1.0f - colorPosition.y / KinectWrapper.Constants.ColorImageHeight;

        var overlayPosition = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
        overlayObject.transform.position = Vector3.Lerp(overlayObject.transform.position, overlayPosition, smoothFactor * Time.deltaTime);
    }

    public IEnumerator<GameObject> GetEnumerator() {
        return _overlays.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}