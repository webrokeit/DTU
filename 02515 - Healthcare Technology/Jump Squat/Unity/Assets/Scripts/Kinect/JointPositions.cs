using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Kinect {
    public class JointPositions {
        public static readonly int JointCount = Enum.GetNames(typeof(Joints)).Length;
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
            get { return _positions[(int)joint]; }
            set {
                _isSet[(int)joint] = true;
                _positions[(int)joint] = value;
            }
        }

        public bool IsSet(Joints joint) {
            return _isSet[(int)joint];
        }

        public void InitIfNotSet(params Joints[] jointsToCheck) {
            InitIfNotSetExt(jointsToCheck);
        }

        public void InitIfNotSetExt(IEnumerable<Joints> jointsToCheckEnumerable) {
            if (jointsToCheckEnumerable == null) return;
            foreach (var joint in jointsToCheckEnumerable.Where(joint => !IsSet(joint))) {
                this[joint] = _manager.GetRawSkeletonJointPos(_userId, (int)joint);
            }
        }

        public bool AreAllNonZero(params Joints[] jointsToCheck) {
            if (jointsToCheck == null || jointsToCheck.Length < 1) return false;
            return jointsToCheck.All(joint => this[joint] != Vector3.zero);
        }

        public bool AreJointsTracked(params Joints[] jointsToCheck) {
            if (jointsToCheck == null || jointsToCheck.Length < 1) return false;
            return jointsToCheck.All(joint => _manager.IsJointTracked(_userId, (int)joint));
        }

        private static readonly Joints[] AvailableJoints = {
            Joints.Hips, Joints.Spine, Joints.Neck, Joints.Head, Joints.LeftShoulder, Joints.LeftElbow, Joints.LeftWrist,
            Joints.LeftHand, Joints.RightShoulder, Joints.RightElbow, Joints.RightWrist, Joints.RightHand, Joints.LeftHip,
            Joints.LeftKnee, Joints.LeftAnkle, Joints.LeftFoot, Joints.RightHip, Joints.RightKnee, Joints.RightAnkle, Joints.RightFoot
        };

        public static IEnumerable<Joints> AllJoints() {
            return AvailableJoints;
        }
    }
}
