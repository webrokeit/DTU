using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Kinect.Gesture.States;
using UnityEngine;

namespace Assets.Scripts.Kinect.Gesture {
    public class JumpGesture : IGesture {
        public Gestures Gesture { get { return Gestures.Jump; } }
        public int Count { get; private set; }
        public float MaxGestureTime { get { return 1.5f; } }

        private readonly Queue<JumpGestureState> _states = new Queue<JumpGestureState>(1000);

        public bool Update(JointPositions positions) {
            var newState = JumpGestureState.Create(positions);
            if (newState != null) _states.Enqueue(newState);

            while (_states.Count > 0 && (Time.time - _states.Peek().StartTime > MaxGestureTime || _states.Peek().Complete)) {
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
}
