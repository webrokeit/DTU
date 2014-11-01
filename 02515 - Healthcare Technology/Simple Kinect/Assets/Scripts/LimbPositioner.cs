using UnityEngine;
using System.Collections;

public class LimbPositioner : MonoBehaviour {
	
	public KUInterface Kinect;
	public KinectWrapper.Joints FromJoint;
	public KinectWrapper.Joints ToJoint;
	
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
		
		//Positions of from and to -objects. Notice we use the "ShoulderCenter" as our origo.
		Vector3 fromPos = Kinect.GetJointPos(FromJoint) - center;
		Vector3 toPos = Kinect.GetJointPos(ToJoint) - center;
		
		//Compute direction and length
		Vector3 direction = (toPos - fromPos);
		float armLength = direction.magnitude;
		
		//Place in the middle between the two points		
		transform.localPosition = fromPos + direction * 0.5f;
		
		//Orient so that we point from one object to the other (Convert from local to world coordinates)
		transform.LookAt(transform.position + 0.5f * direction);
		
		//Scale so we reach the points
		transform.localScale = new Vector3(1,1,armLength);
	
	}
}
