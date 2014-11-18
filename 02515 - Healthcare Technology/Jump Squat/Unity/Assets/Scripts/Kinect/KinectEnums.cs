using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Kinect {
    public enum LogModes {
        None = 0,
        EachFrame,
        GesturesOnly
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
        RightFoot
    }

    public enum Gestures {
        Jump,
        Squat
    }
}
