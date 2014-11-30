using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Kinect.Gesture.States;
using UnityEngine;

namespace Assets.Scripts.Kinect.Gesture {
    public class DoubleClapGesture : IGesture {
        public Gestures Gesture { get { return Gestures.DoubleClap; } }
        public int Count { get; private set; }
        public float MaxGestureTime { get { return 1.5f; } }

        private readonly Queue<DoubleClapGestureState> _states = new Queue<DoubleClapGestureState>(1000);

        public bool Update(JointPositions positions) {
            var newState = DoubleClapGestureState.Create(positions);
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
