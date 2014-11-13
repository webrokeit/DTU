using Assets.Managers;
using UnityEngine;
using System.Collections;

public class LimbPositioner : MonoBehaviour {
	
	public KUInterface Kinect;
	public KinectWrapper.Joints FromJoint;
	public KinectWrapper.Joints ToJoint;
	
	public bool FixateX = true;
	public bool FixateY = true;
	public bool FixateZ = true;
	
	// Use this for initialization
	void Start () {
	    GameEventManager.GameStart += GameStart;
	    GameEventManager.GameOver += GameOver;
	    this.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Choose center
		var center = Kinect.GetJointPos(KinectWrapper.Joints.HIP_CENTER);
		
		//Release any non-fixated directions
		if(!FixateX) center.x = 0;
		if(!FixateY) center.y = 0;
		if(!FixateZ) center.z = 0;
		
		//Positions of from and to -objects. Notice we use the "ShoulderCenter" as our origo.
		var fromPos = Kinect.GetJointPos(FromJoint) - center;
		var toPos = Kinect.GetJointPos(ToJoint) - center;
		
		//Compute direction and length
		var direction = (toPos - fromPos);
		var armLength = direction.magnitude;
		
		//Place in the middle between the two points		
		transform.localPosition = fromPos + direction * 0.5f;
		
		//Orient so that we point from one object to the other (Convert from local to world coordinates)
		transform.LookAt(transform.position + 0.5f * direction);
		
		//Scale so we reach the points
		transform.localScale = new Vector3(1,1,armLength);
	
	}

    private void GameStart() {
        this.enabled = true;
    }

    private void GameOver() {
        this.enabled = false;
    }
}
