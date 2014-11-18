using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Kinect;
using UnityEngine;


public class GestureInputter : MonoBehaviour {
    public KeyCode JumpKey = KeyCode.UpArrow;
    public KeyCode SquatKey = KeyCode.DownArrow;
    
    public delegate void GestureInputEvent(Gestures gesture);
    public event GestureInputEvent GestureInput;

    void Start () { }

    private void Update() {
        if (Input.GetKeyUp(JumpKey)) {
            PublishGesture(Gestures.Jump);
        }
        if (Input.GetKeyUp(SquatKey)) {
            PublishGesture(Gestures.Squat);
        }
    }

    private void PublishGesture(Gestures gesture) {
        if (GestureInput != null) {
            GestureInput(gesture);
        }
    }
}
