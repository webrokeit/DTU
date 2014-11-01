using UnityEngine;
using System.Collections;

public class BrainScriptKinect : MonoBehaviour {
	
	public KUInterface Kinect;
	public GameObject Ball;
	public GameObject RotationObject;
	public GameObject StartZone;
	public Camera ViewCamera;
	public float RotationGain = 0.9f;
	public float MaxRotation = 10f;
	public float CameraGain = 0.3f;
	
	private Quaternion targetRotation;
	private Quaternion defaultRotation;
	private Vector3 CamOffset;

	// Use this for initialization
	void Start () {

		//Save default and current target.
		targetRotation = defaultRotation = RotationObject.transform.rotation;
		//Save camera offset
		CamOffset = ViewCamera.transform.position - Ball.transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
		

		//Calculate UP vector:
		Vector3 UP = Kinect.GetJointPos(KinectWrapper.Joints.SHOULDER_CENTER) - Kinect.GetJointPos(KinectWrapper.Joints.SPINE);
		UP.z = -UP.z; //Mirroring
		//Calculate RIGHT vector
		Vector3 RIGHT = Kinect.GetJointPos(KinectWrapper.Joints.ELBOW_RIGHT) - Kinect.GetJointPos(KinectWrapper.Joints.SHOULDER_RIGHT);
		//Calculate FORWARD vector:
		Vector3 FORWARD = Vector3.Cross(RIGHT,UP);
		
		
		
		Debug.DrawLine(RotationObject.transform.position, RotationObject.transform.position + FORWARD.normalized * 10, Color.blue);
		Debug.DrawLine(RotationObject.transform.position, RotationObject.transform.position + UP.normalized * 10, Color.green);
		Debug.DrawLine(RotationObject.transform.position, RotationObject.transform.position + RIGHT.normalized * 10, Color.red);

		targetRotation = Quaternion.LookRotation(FORWARD, UP);	//Orient the board with the player orientation

		
		Debug.Log(Vector3.Dot(UP.normalized,RIGHT.normalized));
		
		//Smoothly move towards target
		RotationObject.transform.rotation = Quaternion.Lerp(RotationObject.transform.rotation, targetRotation, RotationGain * Time.deltaTime);
		
		//Move camera
		ViewCamera.transform.position = Vector3.Lerp(ViewCamera.transform.position, Ball.transform.position + CamOffset, CameraGain);
	
	}
}
