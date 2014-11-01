using UnityEngine;
using System.Collections;

public class JointPositioner : MonoBehaviour {
	
	public KinectWrapper.Joints JointToTrack;
	public KUInterface Kinect;
	
	public bool fixateX = true;
	public bool fixateY = true;
	public bool fixateZ = true;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		//Choose center
		Vector3 center = Kinect.GetJointPos(KinectWrapper.Joints.SHOULDER_CENTER);
		
		//Release any non-fixated directions
		if(!fixateX)
			center.x = 0;
		if(!fixateY)
			center.y = 0;
		if(!fixateZ)
			center.z = 0;
		
		//Position this object according to chosen joint, but do it relative to the ShoulderCenter position
		transform.localPosition = Kinect.GetJointPos(JointToTrack) - center;
	
	}
}
