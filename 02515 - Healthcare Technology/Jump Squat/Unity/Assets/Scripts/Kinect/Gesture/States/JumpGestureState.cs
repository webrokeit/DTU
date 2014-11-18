using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Kinect.Gesture.States {
    public class JumpGestureState {
        public bool Complete { get; private set; }
        public bool Leaped { get { return LeapedPositions != null; } }

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
}
