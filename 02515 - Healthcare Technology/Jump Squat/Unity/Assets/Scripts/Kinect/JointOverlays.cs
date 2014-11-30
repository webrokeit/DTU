using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Kinect {
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
            var jointPosition = positions[joint];

            var depthPosition = manager.GetDepthMapPosForJointPos(jointPosition);
            var colorPosition = manager.GetColorMapPosForDepthPos(depthPosition);

            var scaleX = colorPosition.x / KinectWrapper.Constants.ColorImageWidth;
            var scaleY = 1.0f - colorPosition.y / KinectWrapper.Constants.ColorImageHeight;

            var distanceToCamera = _overlays[overlayObject];
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
}
