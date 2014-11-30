using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Kinect.Gesture.States {
    public class DoubleClapGestureState {
        public bool Complete { get; private set; }
        public bool Leaped { get { return LeapedPositions != null; } }

        public float StartTime { get; private set; }
        public float LeapedTime { get; private set; }

        public JointPositions StartPositions { get; private set; }
        public JointPositions LeapedPositions { get; private set; }

        private int _claps = 0;
        private bool _handsClosed = false;
        private bool _skip = false;

        private static readonly Joints[] UsedJoints = { Joints.LeftHand, Joints.RightHand };

        private DoubleClapGestureState() {
            Complete = false;
            LeapedTime = 0.000f;
        }

        public bool Update(JointPositions positions) {
            if (_skip || Complete) return Complete;
            positions.InitIfNotSet(UsedJoints);
            if (!positions.AreAllNonZero(UsedJoints)) return false;

            var leftRightDist = Mathf.Abs(positions[Joints.LeftHand].x - positions[Joints.RightHand].x);
            if (!_handsClosed) {
                if (leftRightDist > 0.1f && positions[Joints.RightHand].x < positions[Joints.LeftHand].x) {
                    _skip = true;
                    return false;
                }

                if (leftRightDist < 0.05f && Mathf.Abs(positions[Joints.LeftHand].y - positions[Joints.RightHand].y) < 0.05f) {
                    _handsClosed = true;
                    if (++_claps > 1) Complete = true;
                }
            } else if (leftRightDist > 0.2f){
                _handsClosed = false;
            }

            return Complete;
        }

        public static DoubleClapGestureState Create(JointPositions positions) {
            positions.InitIfNotSet(UsedJoints);
            if (Mathf.Abs(positions[Joints.LeftHand].x - positions[Joints.RightHand].x) < 0.2f) return null;
            if (Mathf.Abs(positions[Joints.LeftHand].y - positions[Joints.RightHand].y) > 0.05f) return null;

            return new DoubleClapGestureState {
                StartTime = Time.time,
                StartPositions = positions
            };
        }
    }
}
