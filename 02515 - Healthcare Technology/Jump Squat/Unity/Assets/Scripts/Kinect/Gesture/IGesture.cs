using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Kinect.Gesture {
    public interface IGesture {
        Gestures Gesture { get; }
        int Count { get; }
        float MaxGestureTime { get; }
        bool Update(JointPositions positions);
    }
}
