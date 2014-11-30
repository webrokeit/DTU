using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Kinect;
using UnityEngine;


public class GestureInputter : MonoBehaviour {
    public KeyCode JumpKey = KeyCode.UpArrow;
    public KeyCode SquatKey = KeyCode.DownArrow;
    public KeyCode StartKey = KeyCode.Space;

    public delegate void GestureInputEvent(Gestures gesture);
    public event GestureInputEvent GestureInput;

    public event Action GameStartRequested;

    void Start () { }

    private void Update() {
        if (Input.GetKeyUp(JumpKey)) {
            if(GestureInput != null) GestureInput(Gestures.Jump);
        }
        if (Input.GetKeyUp(SquatKey)) {
            if (GestureInput != null) GestureInput(Gestures.Squat);
        }
        if (Input.GetKeyUp(StartKey)) {
            if (GameStartRequested != null) GameStartRequested();
        }
    }
}
