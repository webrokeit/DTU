using UnityEngine;
using System.Collections;

public class JointPositioner : MonoBehaviour {
	
	public KinectWrapper.Joints JointToTrack;
	public KUInterface Kinect;
	
	public bool FixateX = true;
	public bool FixateY = true;
	public bool FixateZ = true;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		//Choose center
        var center = Kinect.GetJointPos(KinectWrapper.Joints.HIP_CENTER);
		
		//Release any non-fixated directions
		if(!FixateX) center.x = 0;
		if(!FixateY) center.y = 0;
		if(!FixateZ) center.z = 0;
		
		//Position this object according to chosen joint, but do it relative to the ShoulderCenter position
		transform.localPosition = Kinect.GetJointPos(JointToTrack) - center;
	
	}
}
