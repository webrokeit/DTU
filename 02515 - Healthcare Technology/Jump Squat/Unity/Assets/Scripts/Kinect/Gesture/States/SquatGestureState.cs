using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Kinect.Gesture.States {
    public class SquatGestureState {
        public bool Complete { get; private set; }
        public bool Ducked { get { return DuckedPositions != null; } }

        public float StartTime { get; private set; }
        public float DuckedTime { get; private set; }

        public JointPositions StartPositions { get; private set; }
        public JointPositions DuckedPositions { get; private set; }

        private static readonly Joints[] UsedJoints = { Joints.LeftKnee, Joints.RightKnee, Joints.LeftHip, Joints.RightHip };

        private SquatGestureState() {
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

        public static SquatGestureState Create(JointPositions positions) {
            positions.InitIfNotSet(UsedJoints);
            if (-0.4f > positions[Joints.LeftKnee].y || positions[Joints.LeftKnee].y > -0.2f) return null;
            if (-0.4f > positions[Joints.RightKnee].y || positions[Joints.RightKnee].y > -0.2f) return null;
            if (-0.1f > positions[Joints.LeftHip].y || positions[Joints.LeftHip].y > 0.1f) return null;
            if (-0.1f > positions[Joints.RightHip].y || positions[Joints.RightHip].y > 0.1f) return null;

            return new SquatGestureState {
                StartTime = Time.time,
                StartPositions = positions
            };
        }
    }
}
